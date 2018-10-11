using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// In core XBRL, all items are concepts, but XBRL Dimensions extends items to also include members, hypercubes and dimensions.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Location nodes link to items via the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The XName of the item, consisting of a namespace and a local name, e.g. {http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}CashAndCashEquivalents.
        /// </summary>
        /// <remarks>
        /// The namespace is determined via the targetNamespace attribute in the schema.
        /// </remarks>
        public XName Name { get; set; }

        /// <summary>
        /// The item type, e.g. item (=concept or member), hypercube, dimension.
        /// </summary>
        public string SubstitutionGroup { get; set; }

        #region Shared properties (provided for all item types, even though they have no meaning for some types)

        public bool IsAbstract { get; set; }

        public string BalanceType { get; set; }

        public string DataType { get; set; }

        public string PeriodType { get; set; }

        #endregion


        public IList<LocatorNode> Nodes { get; }

        public Schema Schema { get; internal set; }

        public Item()
        {
            Nodes=new List<LocatorNode>();
        }

        public Item(XElement xItem, XNamespace targetNamespace)
        {
            Nodes = new List<LocatorNode>();

            Id = xItem.Attribute(ElementXNames.Id).Value;
            Name = targetNamespace + xItem.Attribute(ElementXNames.Name).Value;
            SubstitutionGroup = xItem.Attribute(ElementXNames.SubstitutionGroup).Value;
            IsAbstract = xItem.Attribute(ElementXNames.Abstract) == null ? false : bool.Parse(xItem.Attribute(ElementXNames.Abstract).Value);
            BalanceType = xItem.Attribute(ElementXNames.Balance)?.Value;
            PeriodType = xItem.Attribute(ElementXNames.PeriodType).Value;
            DataType = xItem.Attribute(ElementXNames.Type).Value;
        }

        public string FindLabel(Uri preferredLabelRole)
        {
            var labelNodes = Nodes.SelectMany(n => n.FromArcs.OfType<LabelArc>()).Select(la => la.ToLabelNode).ToList();

            var preferredLabelNode = labelNodes.FirstOrDefault(a => a.Role == preferredLabelRole);
            if (preferredLabelNode != null)
            {
                return preferredLabelNode.Text;
            }

            var standardLabelNode = labelNodes.FirstOrDefault(a => a.Role == LabelRoles.Standard);
            if (standardLabelNode != null)
            {
                return standardLabelNode.Text;
            }

            throw new InvalidOperationException($"Could not find a label for the item {Name}.");
        }

        /// <summary>
        /// The text of the related documentation label, if any.
        /// </summary>
        /// <returns>null if there is no related documentation label.</returns>
        public string GetDocumentation()
        {
            var labelNodes = Nodes.SelectMany(n => n.FromArcs.OfType<LabelArc>()).Select(la => la.ToLabelNode).ToList();

            var documentationLabelNode = labelNodes.FirstOrDefault(a => a.Role == LabelRoles.Documentation);
            return documentationLabelNode?.Text;
        }

        public IList<ReferenceNode> GetReferenceNodes()
        {
            return Nodes.SelectMany(n => n.FromArcs.OfType<ReferenceArc>()).Select(la => la.ToReference).ToList();
        }

        public Uri GetReference()
        {
            return Schema.CreateReference(Id);
        }

        public XElement ToXml()
        {
            var xElement = new XElement(ElementXNames.Element,
                new XAttribute(ElementXNames.Abstract, IsAbstract.ToString().ToLowerInvariant()),
                new XAttribute(ElementXNames.Id, Id),
                new XAttribute(ElementXNames.Name, Name.LocalName),
                new XAttribute(ElementXNames.Nillable, "true"),
                new XAttribute(ElementXNames.SubstitutionGroup, SubstitutionGroup),
                new XAttribute(ElementXNames.Type, DataType),
                new XAttribute(ElementXNames.PeriodType, PeriodType)
                );

            // A small (about 150) number of IFRS concepts are monetary and still lack a balance type. This occurs, e.g. for prices.
            if (DataType == DataTypeRegistry.Monetary && BalanceType != null)
            {
                xElement.Add(new XAttribute(ElementXNames.Balance, BalanceType));
            }

            return xElement;
        }
    }
}