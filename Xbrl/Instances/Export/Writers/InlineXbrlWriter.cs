using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances.Export.Presentation;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public class InlineXbrlWriter : PresenterBasedInstanceWriter
    {
        // Substitution markers. The writer is looking for elements with these HTML ids.
        private const string IxbrlHeaderId = "ixbrl-header";
        private const string EntityId = "entity";
        private const string CurrencyId = "currency";
        private const string ScaleId = "scale";
        private const string NetworksId = "networks";

        private readonly Stack<XmlBasedTableContext> _activeTables;
        private readonly string _numberFormat;
        private readonly InlineXbrlWriterSettings _settings;

        private readonly XDocument _template;

        private XElement _xActiveNetwork;
        private XElement _xBody;
        private XElement _xHead;

        private XElement _xHtml;
        private XElement _xNetworksSection;

        /// <summary>
        /// The result of the write operation.
        /// </summary>
        public XDocument Document { get; private set; }

        public InlineXbrlWriter(XDocument template, InlineXbrlWriterSettings settings)
        {
            Scope = settings.Scope;

            _template = template;
            _settings = settings;
            _numberFormat = GetNumberFormat(settings.Culture);

            ValidateTemplate(_template);

            _activeTables = new Stack<XmlBasedTableContext>();
        }

        private void RemoveDocType()
        {
            var first = Document.FirstNode;
            if (first.NodeType == XmlNodeType.DocumentType)
            {
                first.Remove();
            }
        }

        private void ValidateTemplate(XDocument document)
        {
            _xHtml = document.Root;
            if (_xHtml == null || _xHtml.Name != HtmlXNames.Html)
            {
                throw new InstanceExportException("The provided template lacks an html root element.");
            }

            _xHead = _xHtml.Element(HtmlXNames.Head);
            if (_xHead == null)
            {
                throw new InstanceExportException("The provided template lacks a head element.");
            }

            _xBody = _xHtml.Element(HtmlXNames.Body);
            if (_xBody == null)
            {
                throw new InstanceExportException("The provided template lacks a body element.");
            }

            _xNetworksSection = _xBody.FindById(NetworksId);
            if (_xNetworksSection == null)
            {
                throw new InstanceExportException($"The provided template lacks a marker element for the presentation networks. Add an element with id '{NetworksId}'.");
            }
        }

        private void SetTitle(string title)
        {
            var xTitle = _xHead.Element(HtmlXNames.Title);
            if (xTitle == null)
            {
                xTitle = new XElement(HtmlXNames.Title);
                _xHead.AddFirst(xTitle);
            }

            xTitle.Value = title;
        }

        private void SetNamespaceDeclarations(Instance instance)
        {
            var standardNamespaceDeclarations = new Dictionary<string, XNamespace>
            {
                {"xsd", Namespaces.Xsd},
                {"xsi", Namespaces.Xsi},
                {"ix", Namespaces.Ix},
                {"ixt", Namespaces.Ixt},
                {"link", Namespaces.Link},
                {"xlink", Namespaces.XLink},
                {"xbrli", Namespaces.Xbrli},
                {"iso4217", Namespaces.Iso4217},
                {"xbrldi", Namespaces.Xbrldi},
                {"xbrldt", Namespaces.Xbrldt}
            };
            foreach (var namespaceDeclaration in standardNamespaceDeclarations)
            {
                _xHtml.Add(new XAttribute(XNamespace.Xmlns + namespaceDeclaration.Key, namespaceDeclaration.Value));
            }

            instance.Dts.AddItemNamespaceDeclarations(_xHtml);
        }

        private void SetIxbrlHeader(Instance instance)
        {
            var xHeaderContainer = _xBody.FindById(IxbrlHeaderId);
            if (xHeaderContainer==null)
            {
                throw new InvalidOperationException($"The provided template lacks a marker element for the iXBRL header. Add an element with id '{IxbrlHeaderId}' where the iXBRL header should appear.");
            }

            xHeaderContainer.Add(instance.GetIxbrlHeaderElement(_xHtml));
        }

        private void WriteTabularConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts)
        {
            var tableContext = _activeTables.Peek();
            if (tableContext.Table == null)
            {
                tableContext.Table = CreateHtmlTable(tableContext);
            }

            var xBodyTr = new XElement(HtmlXNames.Tr);
            var xLabelTd = new XElement(HtmlXNames.Td, CreateMemberElement(conceptMember, HtmlXNames.Div));
            xBodyTr.Add(xLabelTd);

            foreach (var member in tableContext.LinearisedHorizontalAxis)
            {
                var xValueTd = new XElement(HtmlXNames.Td);

                var fact = facts[member];
                if (fact != null)
                {
                    xValueTd.Add(CreateFactElement(fact, conceptMember));
                }

                xBodyTr.Add(xValueTd);
            }

            var xTBody = tableContext.Table.Element(HtmlXNames.Tbody);
            xTBody.Add(xBodyTr);
        }

        private XElement CreateMemberElement(Member member, XName containerName)
        {
            var xSpan = new XElement(containerName);
            xSpan.AddClass(InlineXbrlClassNames.Member);
            xSpan.AddClass(string.Format(InlineXbrlClassNames.Level, member.Depth));

            var itemMember = member as ItemMember;
            if (itemMember!=null)
            {
                PopulateItemMemberElement(xSpan, itemMember);
            }
            else
            {
                xSpan.Value = member.ToString(_settings.Culture);
            }

            return xSpan;
        }

        private void PopulateItemMemberElement(XElement xContainer, ItemMember itemMember)
        {
            xContainer.Value = RemoveBracketedMarkers(itemMember.Label);
            xContainer.AddClass(InlineXbrlClassNames.ItemMember);

            if (itemMember.Item.IsAbstract)
            {
                xContainer.AddClass(InlineXbrlClassNames.AbstractMember);
            }

            var conceptMember = itemMember as ConceptMember;
            if (conceptMember != null && LabelRoles.Totals.Contains(conceptMember.PreferredLabelRole))
            {
                xContainer.AddClass(InlineXbrlClassNames.TotalMember);
            }

            xContainer.AddData("info", SerializeJson(CreateItemMemberInfo(itemMember)));
        }

        private ItemMemberInfo CreateItemMemberInfo(ItemMember itemMember)
        {
            var info = new ItemMemberInfo
            {
                name = itemMember.Item.Name.ToColonSeparated(_xHtml),
                doc = _settings.IncludeDocumentation ? itemMember.Item.GetDocumentation() : null,
                refs = _settings.IncludeReferences ? CreateReferenceInfos(itemMember) : null
            };
            return info;
        }

        private IList<ReferenceInfo> CreateReferenceInfos(ItemMember itemMember)
        {
            var knownRoles = new Dictionary<Uri, string> {
                [ReferenceRoles.Disclosure] = "disclosure",
                [ReferenceRoles.Example] = "example",
                [ReferenceRoles.CommonPractice] = "commonpractice",
            };

            var referenceNodes = itemMember.Item.GetReferenceNodes();
            return referenceNodes.Select(rn =>
                new ReferenceInfo
                {
                    type = knownRoles[rn.Role],
                    loc = rn.ToString(),
                    link = rn.Uri
                }).ToList();
        }

        public static string SerializeJson<TDataContract>(TDataContract t)
        {
            using (var stream = new MemoryStream())
            {
                var ds = new DataContractJsonSerializer(typeof(TDataContract));
                ds.WriteObject(stream, t);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private string RemoveBracketedMarkers(string label)
        {
            var regEx = new Regex(@"\[[a-z\s]+\]");
            return regEx.Replace(label, string.Empty).Trim();
        }

        private XElement CreateFactElement(FactModel factModel, ConceptMember conceptMember)
        {
            var item = conceptMember.Item;

            var xSpan = new XElement(HtmlXNames.Span);
            xSpan.AddClass(InlineXbrlClassNames.Fact);

            if (item.DataType == DataTypeRegistry.Monetary)
            {
                CreateNonFractionFactElement(xSpan, factModel.Fact, conceptMember);
            }
            else if (DataTypeRegistry.Textual.Contains(item.DataType))
            {
                CreateTextualFactElement(xSpan, factModel.Fact, item);
            }

            return xSpan;
        }

        private void CreateNonFractionFactElement(XElement element, Fact fact, ConceptMember conceptMember)
        {
            var item = conceptMember.Item;

            element.AddClass(InlineXbrlClassNames.NonFractionFact);

            // Values in iXBRL are always absolute and a sign attribute is used to express a negative fact value.
            // This is for two reasons:
            // 1. There are several ways negative amounts can be rendered, e.g. leading dash, brackets, colour-coded.
            // 2. With negated labels, the sign of the fact value is the opposite of what is displayed.
            var accountingValue = fact.GetAccountingValue(item.BalanceType);

            var scaledAbsoluteValue = Math.Abs(accountingValue) / Math.Pow(10, _settings.Scale);
            var formattedValue = scaledAbsoluteValue.ToString("N", _settings.Culture);

            var isAccoutingValueNegative = accountingValue < 0;
            var decimals = _settings.Culture.NumberFormat.NumberDecimalDigits;

            var xNonFraction = new XElement(Namespaces.Ix + "nonFraction");
            xNonFraction.SetAttributeValue("name", item.Name.ToColonSeparated(_xHtml));
            xNonFraction.SetAttributeValue("contextRef", fact.GetContextId(_xHtml));
            xNonFraction.SetAttributeValue("unitRef", fact.Unit.Id);
            xNonFraction.SetAttributeValue("scale", _settings.Scale);
            xNonFraction.SetAttributeValue("decimals", decimals);
            xNonFraction.SetAttributeValue("format", $"ixt:{_numberFormat}");
            if (isAccoutingValueNegative)
            {
                xNonFraction.SetAttributeValue("sign", "-");
            }

            xNonFraction.Value = formattedValue;
            element.Add(xNonFraction);

            var isDisplayNegative = LabelRoles.Negatives.Contains(conceptMember.PreferredLabelRole)
                ? !isAccoutingValueNegative
                : isAccoutingValueNegative;
            element.AddClass(isDisplayNegative ? InlineXbrlClassNames.NegativeFact : InlineXbrlClassNames.PositiveFact);
            if (isDisplayNegative)
            {
                element.AddFirst("(");
                element.Add(")");
            }
        }

        // TODO: inline HTML inside text blocks.
        private void CreateTextualFactElement(XElement element, Fact fact, Item item)
        {
            element.AddClass(InlineXbrlClassNames.NonNumericFact);

            var xNonNumeric = new XElement(Namespaces.Ix + "nonNumeric");
            xNonNumeric.SetAttributeValue("name", item.Name.ToColonSeparated(_xHtml));
            xNonNumeric.SetAttributeValue("contextRef", fact.GetContextId(_xHtml));
            xNonNumeric.Value = fact.Value;

            element.Add(xNonNumeric);
        }

        private XElement CreateHtmlTable(XmlBasedTableContext tableContext)
        {
            var xColGroup = new XElement(HtmlXNames.Colgroup);
            xColGroup.Add(new XElement(HtmlXNames.Col, new XAttribute("class", "concept")));

            var xHeaderTr = new XElement(HtmlXNames.Tr, new XElement(HtmlXNames.Th));
            foreach (var member in tableContext.LinearisedHorizontalAxis)
            {
                xColGroup.Add(new XElement(HtmlXNames.Col, new XAttribute("class", "fact")));
                var xTh = new XElement(HtmlXNames.Th, CreateMemberElement(member, HtmlXNames.Div));
                xHeaderTr.Add(xTh);
            }

            var xTable = new XElement(HtmlXNames.Table);
            xTable.AddData("horizontal-axis-role", tableContext.HorizontalAxis.Name.LocalName);
            xTable.AddClass(InlineXbrlClassNames.Table);
            xTable.Add(xColGroup);
            xTable.Add(new XElement(HtmlXNames.Thead, xHeaderTr));
            xTable.Add(new XElement(HtmlXNames.Tbody));
            _xActiveNetwork.Add(xTable);

            return xTable;
        }

        private void WriteTextBlockConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts)
        {
            // To create a new table after the text block.
            var tableContext = _activeTables.Peek();
            tableContext.Table = null;

            var xConceptH2 = CreateMemberElement(conceptMember, HtmlXNames.H2);
            _xActiveNetwork.Add(xConceptH2);

            var occupiedMembers = facts.Where(item => item.Value != null).ToList();
            if (!occupiedMembers.Any())
            {
                return;
            }

            var xValueDiv = new XElement(HtmlXNames.Div);
            xValueDiv.AddClass(InlineXbrlClassNames.Fact);
            xValueDiv.AddClass(string.Format(InlineXbrlClassNames.Level, conceptMember.Depth));

            _xActiveNetwork.Add(xValueDiv);

            if (occupiedMembers.Count > 1)
            {
                xValueDiv.Add($"Found {occupiedMembers.Count} facts for this text block, but there should be at most one.");
                return;
            }

            var singleFact = occupiedMembers.First().Value;
            xValueDiv.Add(CreateFactElement(singleFact, conceptMember));
        }

        private string GetNumberFormat(CultureInfo culture)
        {
            var numberFormatInfo = culture.NumberFormat;

            if (numberFormatInfo.NumberDecimalSeparator == "." && numberFormatInfo.NumberGroupSeparator == ",")
            {
                return "numcommadot";
            }

            if (numberFormatInfo.NumberDecimalSeparator == "," && numberFormatInfo.NumberGroupSeparator == ".")
            {
                return "numdotcomma";
            }

            if (numberFormatInfo.NumberDecimalSeparator == "." && numberFormatInfo.NumberGroupSeparator == " ")
            {
                return "numspacedot";
            }

            if (numberFormatInfo.NumberDecimalSeparator == "," && numberFormatInfo.NumberGroupSeparator == " ")
            {
                return "numspacecomma";
            }

            if (numberFormatInfo.NumberDecimalSeparator == "." && numberFormatInfo.NumberGroupSeparator == " ")
            {
                return "numspacedot";
            }

            if (numberFormatInfo.NumberDecimalSeparator == "," && string.IsNullOrEmpty(numberFormatInfo.NumberGroupSeparator))
            {
                return "numcomma";
            }

            throw new ArgumentException("The culture specifies a number format that is not supported by iXBRL.");
        }

        private void RemoveAllScriptsAndExternals(XDocument document)
        {
            var removables = document.Descendants().Where(e => e.Name == HtmlXNames.Script || e.Name == HtmlXNames.Link).ToList();
            foreach (var removable in removables)
            {
                removable.Remove();
            }
        }

        #region IPresenterWriter

        public override void WriteBeginExport(Instance instance)
        {
            Document = new XDocument(_template);
            ValidateTemplate(Document);
            RemoveDocType();
            if (_settings.ForFiling)
            {
                RemoveAllScriptsAndExternals(Document);
            }

            SetTitle(_settings.Title);
            SetNamespaceDeclarations(instance);
            SetIxbrlHeader(instance);
        }

        public override void WriteIntro(Entity entity, CurrencyUnit unit)
        {
            _xBody.Substitute(EntityId, entity.Name);
            _xBody.Substitute(CurrencyId, unit.Iso4217Code);

            var scaleText = Math.Pow(10, _settings.Scale).ToString("N0", _settings.Culture);
            _xBody.Substitute(ScaleId, scaleText);
        }

        public override void WriteBeginPresentationNetwork(PresentationNetwork presentationNetwork)
        {
            _xActiveNetwork = new XElement(HtmlXNames.Article);
            _xActiveNetwork.AddData("role", presentationNetwork.Role.AbsoluteUri);
            _xActiveNetwork.AddClass(InlineXbrlClassNames.Network);
            _xNetworksSection.Add(_xActiveNetwork);

            var xHeader = new XElement(HtmlXNames.Header,
                new XElement(HtmlXNames.H1, presentationNetwork.Name));
            _xActiveNetwork.Add(xHeader);
        }

        public override void WriteBeginTable(Axis horizontalAxis)
        {
            var tableContext = new XmlBasedTableContext
            {
                HorizontalAxis = horizontalAxis
            };
            _activeTables.Push(tableContext);
        }

        public override void WriteConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts)
        {
            var concept = conceptMember.Item;

            var isTextBlock = concept.DataType == DataTypeRegistry.TextBlock;
            var requiresHtmlTable = concept.IsAbstract || !isTextBlock;

            if (requiresHtmlTable)
            {
                WriteTabularConcept(conceptMember, facts);
            }
            else
            {
                WriteTextBlockConcept(conceptMember, facts);
            }
        }

        public override void WriteEndTable()
        {
            _activeTables.Pop();
        }

        public override void WriteEndPresentationNetwork()
        {
        }

        public override void WriteEndExport()
        {
        }

        #endregion


        class XmlBasedTableContext : TableContext<XElement>
        {

        }

    }
}