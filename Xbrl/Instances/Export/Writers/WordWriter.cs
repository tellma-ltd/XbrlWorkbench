using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Banan.Tools.Xbrl.Instances.Export.Presentation;
using Banan.Tools.Xbrl.Taxonomies.Constants;
using Xceed.Words.NET;
using Axis = Banan.Tools.Xbrl.Instances.Export.Presentation.Axis;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public class WordWriter : PresenterBasedInstanceWriter
    {

        static class Placeholders
        {
            public const string Entity = "entity";
            public const string Currency = "currency";
            public const string Scale = "scale";

            public const string Network = "network";

            public const string CellMember = "cell-member";
            public const string CellMemberLevel = "cell-member-level{0}";
            public const string CellConceptLevel = "cell-concept-level{0}";
            public const string CellFactMonetaryPositive = "cell-fact-monetary-positive";
            public const string CellFactMonetaryNegative = "cell-fact-monetary-negative";
            public const string CellFactString = "cell-fact-string";

            public const string TextblockConceptLevel = "textblock-concept-level{0}";
            public const string TextblockFactLevel = "textblock-fact-level{0}";

            public static IEnumerable<string> AllParagraphs()
            {
                var singles = new[]
                {
                    Network,
                };
                var leveled = new[] {TextblockConceptLevel, TextblockFactLevel};
                return singles.Union(Enumerable.Range(1, 9).SelectMany(i => leveled.Select(l => string.Format(l, i))));
            }
        }

        // Placeholders.

        private readonly WordWriterSettings _settings;

        private readonly byte[] _templateBytes;
        private DocX _document;

        private readonly Stack<DocXBasedTableContext> _activeTables;
        private InsertBeforeOrAfter _activeInsertBeforeOrAfter;

        private Table _templateTable;

        /// <summary>
        /// The result of the write operation.
        /// </summary>
        public byte[] DocumentBytes { get; private set; }

        public WordWriter(byte[] template, WordWriterSettings settings)
        {
            Scope = settings.Scope;
            _settings = settings;

            _templateBytes = template;

            _activeTables = new Stack<DocXBasedTableContext>();
        }

        private void WriteTabularConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts)
        {
            var tableContext = _activeTables.Peek();
            if (tableContext.Table == null)
            {
                tableContext.Table = AddTable(tableContext);
            }

            var conceptRow = tableContext.Table.InsertRow();
            var placeholder = string.Format(Placeholders.CellConceptLevel, conceptMember.Depth);
            AddCellParagraph(conceptRow.Cells.First(), placeholder, GetMemberText(conceptMember));

            var iColumn = 1;
            foreach (var member in tableContext.LinearisedHorizontalAxis)
            {
                var cell = conceptRow.Cells[iColumn];

                var fact = facts[member];
                if (fact != null)
                {
                    AddCellFact(cell, fact, conceptMember);
                }

                iColumn += 1;
            }
        }

        private string GetMemberText(Member member)
        {
            var itemMember = member as ItemMember;
            if (itemMember != null)
            {
                return RemoveBracketedMarkers(itemMember.Label);
            }
            else
            {
                return member.ToString(_settings.Culture);
            }
        }

        private string RemoveBracketedMarkers(string label)
        {
            var regEx = new Regex(@"\[[a-z\s]+\]");
            return regEx.Replace(label, string.Empty).Trim();
        }

        private Paragraph AddCellFact(Cell cell, FactModel factModel, ConceptMember conceptMember)
        {
            var item = conceptMember.Item;

            if (item.DataType == DataTypeRegistry.Monetary)
            {
                return AddNonFractionCellFact(cell, factModel.Fact, conceptMember);
            }

            if (DataTypeRegistry.Textual.Contains(item.DataType))
            {
                return AddTextualCellFact(cell, factModel.Fact);
            }

            return null;
        }

        private Paragraph AddNonFractionCellFact(Cell cell, Fact fact, ConceptMember conceptMember)
        {
            var item = conceptMember.Item;

            var accountingValue = fact.GetAccountingValue(item.BalanceType);
            var scaledValue = accountingValue / Math.Pow(10, _settings.Scale);
            var displayValue = LabelRoles.Negatives.Contains(conceptMember.PreferredLabelRole) ? -scaledValue : scaledValue;
            var formattedAbsoluteValue = Math.Abs(displayValue).ToString("N", _settings.Culture);

            var displayText = displayValue < 0 ? $"({formattedAbsoluteValue})" : formattedAbsoluteValue;
            var placeholder = displayValue < 0 ? Placeholders.CellFactMonetaryNegative : Placeholders.CellFactMonetaryPositive;

            return AddCellParagraph(cell, placeholder, displayText);
        }

        // TODO: inline HTML inside text blocks.
        private Paragraph AddTextualCellFact(Cell cell, Fact fact)
        {
            return AddCellParagraph(cell, Placeholders.CellFactString, fact.Value);
        }

        private Table AddTable(DocXBasedTableContext tableContext)
        {
            if (_activeInsertBeforeOrAfter is Table)
            {
                _activeInsertBeforeOrAfter = _activeInsertBeforeOrAfter.InsertParagraphAfterSelf(string.Empty);
            }

            var table = _activeInsertBeforeOrAfter.InsertTableAfterSelf(_templateTable);
            _activeInsertBeforeOrAfter = table;

            while (table.RowCount > 1)
            {
                table.RemoveRow();
            }
            while (table.ColumnCount > 1)
            {
                table.RemoveColumn();
            }

            var headerRow = table.Rows.First();

            foreach (var member in tableContext.LinearisedHorizontalAxis)
            {
                table.InsertColumn();

                var itemMember = member as ItemMember;
                var placeholder = itemMember != null ? string.Format(Placeholders.CellMemberLevel, itemMember.Depth) : Placeholders.CellMember;
                var cell = headerRow.Cells.Last();
                cell.VerticalAlignment = VerticalAlignment.Bottom; // Hardcoded. We could read it from the template if we wanted.
                AddCellParagraph(cell, placeholder, GetMemberText(member));
            }

            return table;
        }

        private void WriteTextBlockConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts)
        {
            // To start a new table after the text block.
            FinishActiveTable();

            // Concept
            var conceptPlaceholder = string.Format(Placeholders.TextblockConceptLevel, conceptMember.Depth);
            AddTextblockParagraph(conceptPlaceholder, GetMemberText(conceptMember));

            var occupiedMembers = facts.Where(item => item.Value != null).ToList();
            if (!occupiedMembers.Any())
            {
                return;
            }

            // Fact
            var factValue = occupiedMembers.Count > 1 
                ? $"Found {occupiedMembers.Count} facts for this text block, but there should be at most one." 
                : occupiedMembers.First().Value.Fact.Value;

            var factPlaceholder = string.Format(Placeholders.TextblockFactLevel, conceptMember.Depth);
            AddTextblockParagraph(factPlaceholder, factValue);
        }

        private void AddTextblockParagraph(string placeholder, string substitutionText)
        {
            var placeholderParagraph = FindPlaceholderParagraph(_document, placeholder);
            if (placeholderParagraph == null)
            {
                throw new InstanceExportException($"The provided template lacks a placeholder '{GetFullPlaceholderText(placeholder)}'.");
            }

            var newParagraph = _activeInsertBeforeOrAfter.InsertParagraphAfterSelf(placeholderParagraph);
            Substitute(newParagraph, placeholder, substitutionText);

            _activeInsertBeforeOrAfter = newParagraph;
        }

        private Paragraph AddCellParagraph(Cell cell, string placeholder, string substitutionText)
        {
            var placeholderParagraph = FindPlaceholderParagraph(_templateTable, placeholder);
            if (placeholderParagraph == null)
            {
                throw new InstanceExportException($"The provided template table lacks a cell with the placeholder '{GetFullPlaceholderText(placeholder)}'.");
            }

            cell.RemoveParagraph(cell.Paragraphs[0]);
            var newParagraph = cell.InsertParagraph(placeholderParagraph);
            Substitute(newParagraph, placeholder, substitutionText);

            return newParagraph;
        }


        private void Substitute(Container container, string placeholder, string replacement)
        {
            container.ReplaceText(GetFullPlaceholderText(placeholder), replacement);
        }

        private void Substitute(Paragraph paragraph, string placeholder, string replacement)
        {
            paragraph.ReplaceText(GetFullPlaceholderText(placeholder), replacement);
        }


        private string GetFullPlaceholderText(string placeholder)
        {
            return $"{_settings.PlaceholderPrefix}{placeholder}{_settings.PlaceholderSuffix}";
        }

        private Paragraph FindPlaceholderParagraph(Container container, string placeholder)
        {
            return FindPlaceholderParagraph(container.Paragraphs, placeholder);
        }
        private Paragraph FindPlaceholderParagraph(Table table, string placeholder)
        {
            return FindPlaceholderParagraph(table.Paragraphs, placeholder);
        }
        private Paragraph FindPlaceholderParagraph(IEnumerable<Paragraph> paragraphs, string placeholder)
        {
            return paragraphs.FirstOrDefault(p => p.Text.Contains(GetFullPlaceholderText(placeholder)));
        }

        private Table FindTemplateTable(string placeholder)
        {
            var templateTable = _document.Tables
                .FirstOrDefault(t => t.Rows.SelectMany(r => r.Cells).Any(c => c.FindAll(Placeholders.CellMember).Any()));

            if (templateTable == null)
            {
                throw new InstanceExportException($"Could not find a table with a placeholder '{GetFullPlaceholderText(placeholder)}' inside.");
            }

            return templateTable;
        }

        private void RemovePlaceholders()
        {
            foreach (var placeholder in Placeholders.AllParagraphs())
            {
                var placeholderParagraph = FindPlaceholderParagraph(_document, placeholder);
                placeholderParagraph?.Remove(false);
            }
            _templateTable.Remove();
        }

        private void FinishActiveTable()
        {
            var tableContext = _activeTables.Peek();
            if (tableContext.Table != null)
            {
                tableContext.Table.AutoFit = AutoFit.Contents;
                tableContext.Table = null;
            }
        }


        #region IPresenterWriter

        public override void WriteBeginExport(Instance instance)
        {
            using (var memoryStream = new MemoryStream(_templateBytes))
            {
                _document = DocX.Load(memoryStream);
            }

            _templateTable = FindTemplateTable(Placeholders.CellMember);
        }

        public override void WriteIntro(Entity entity, CurrencyUnit unit)
        {
            Substitute(_document, Placeholders.Entity, entity.Name);
            Substitute(_document, Placeholders.Currency, unit.Iso4217Code);

            var scaleText = Math.Pow(10, _settings.Scale).ToString("N0", _settings.Culture);
            Substitute(_document, Placeholders.Scale, scaleText);
        }

        public override void WriteBeginPresentationNetwork(PresentationNetwork presentationNetwork)
        {
            var placeholderParagraph = FindPlaceholderParagraph(_document, Placeholders.Network);
            if (placeholderParagraph == null)
            {
                throw new InstanceExportException($"The provided template lacks a placeholder for the presentation network. Add a placeholder that looks like this: '{GetFullPlaceholderText(Placeholders.Network)}'.");
            }

            if (_activeInsertBeforeOrAfter == null)
            {
                _activeInsertBeforeOrAfter = placeholderParagraph;
            }

            var newParagraph = _activeInsertBeforeOrAfter.InsertParagraphAfterSelf(placeholderParagraph);
            Substitute(newParagraph, Placeholders.Network, presentationNetwork.Name);

            _activeInsertBeforeOrAfter = newParagraph;
        }

        public override void WriteBeginTable(Axis horizontalAxis)
        {
            var tableContext = new DocXBasedTableContext
            {
                HorizontalAxis = horizontalAxis
            };
            _activeTables.Push(tableContext);
        }

        public override void WriteConcept(ConceptMember conceptMember, IDictionary<Member, FactModel> facts)
        {
            var concept = conceptMember.Item;

            var isTextBlock = concept.DataType == DataTypeRegistry.TextBlock;
            var requiresTable = concept.IsAbstract || !isTextBlock;

            if (requiresTable)
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
            FinishActiveTable();

            _activeTables.Pop();
        }

        public override void WriteEndPresentationNetwork()
        {
        }

        public override void WriteEndExport()
        {
            RemovePlaceholders();

            using (var memoryStream = new MemoryStream())
            {
                _document.SaveAs(memoryStream);
                DocumentBytes = memoryStream.ToArray();
            }
        }


        #endregion

        class DocXBasedTableContext : TableContext<Table>
        {

        }

    }
}