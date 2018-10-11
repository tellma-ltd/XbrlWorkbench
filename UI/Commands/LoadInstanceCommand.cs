using System;
using Unity.Attributes;
using OfficeOpenXml;
using System.IO;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Load-Instance", CommandType = typeof(LoadInstanceCommand))]
    public class LoadInstanceCommand : ShellCommandBase
    {
        [Dependency]
        public Instance Instance { get; set; }

        public override void Invoke()
        {
            base.Invoke();

            XNamespace bSharpNamespace = "http://banan-it.com/taxonomy/2018-07-05/bsharp";
            XNamespace ifrsNamespace = "http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full";

            var xDeclarationsHolder = new XElement("dummy", 
                new XAttribute(XNamespace.Xmlns + "ifrs-full", ifrsNamespace),
                new XAttribute(XNamespace.Xmlns + "banan", bSharpNamespace));

            Instance.Entities.Clear();
            Instance.Periods.Clear();
            Instance.Units.Clear();
            Instance.Facts.Clear();

            FillFromExcelDocument(@"C:\Data\Banan\TFS\banantfs\Scrum\Tools\XbrlWorkbench\UI\Samples\BSharpIfrsFactsValuesSampleV1.xlsx", xDeclarationsHolder);

            Context.Logger.WriteLine("Loaded.");
        }

        private void FillFromExcelDocument(string fileName, XElement xDeclarationsHolder)
        {
            var etbUnit = new CurrencyUnit("ETB");
            Instance.Units.Add(etbUnit);

            var quarter1Of2018 = new DurationPeriod(new DateTime(2018, 1, 1), new DateTime(2018, 3, 31));
            var startOfQuarter1Of2018 = new InstantPeriod(new DateTime(2017, 12, 31));
            var endOfQuarter1Of2018 = new InstantPeriod(new DateTime(2018, 3, 31));
            Instance.Periods.Add(quarter1Of2018);
            Instance.Periods.Add(startOfQuarter1Of2018);
            Instance.Periods.Add(endOfQuarter1Of2018);

            var wsi = new Entity
            {
                Id = "wsi",
                IdentifierScheme = new Uri("http://www.sec.gov/CIK"),
                Identifier = "WSI",
                Name = "Walia Steel Industry PLC"
            };
            Instance.Entities.Add(wsi);

            var package = new ExcelPackage(new FileInfo(fileName));
            var worksheet = package.Workbook.Worksheets[1];

            ExcelRangeBase conceptCell = worksheet.Cells["B2"];
            ExcelRangeBase valueCell = worksheet.Cells["C2"];
            ExcelRangeBase startDateCell = worksheet.Cells["D2"];
            ExcelRangeBase dateOrEndDateCell = worksheet.Cells["E2"];
            ExcelRangeBase explicitMembersCell = worksheet.Cells["F2"];

            while (conceptCell.Value!=null && valueCell.Value != null)
            {
                var concept = conceptCell.GetValue<string>();
                var value = valueCell.Value;
                var period = startDateCell.Value==null || string.IsNullOrEmpty(startDateCell.Value.ToString()) ?
                    new InstantPeriod(dateOrEndDateCell.GetValue<DateTime>()) :
                    (Period)new DurationPeriod(startDateCell.GetValue<DateTime>(), dateOrEndDateCell.GetValue<DateTime>());

                var conceptName = ToXName(concept, xDeclarationsHolder);
                var fact = new Fact (conceptName, wsi, period) { 
                    Unit = etbUnit,
                    Value = value.ToString(),
                    BalanceMethod = BalanceMethod.Algebraic
                };

                AddExplicitMembers(fact, explicitMembersCell.Value, xDeclarationsHolder);

                Instance.Facts.Add(fact);

                conceptCell = conceptCell.Offset(1, 0);
                valueCell = valueCell.Offset(1, 0);
                startDateCell = startDateCell.Offset(1, 0);
                dateOrEndDateCell = dateOrEndDateCell.Offset(1, 0);
                explicitMembersCell = explicitMembersCell.Offset(1, 0);
            }

        }

        private void AddExplicitMembers(Fact fact, object cellValue, XElement namespaceDeclarationsElement)
        {
            if (cellValue==null)
            {
                return;
            }

            var parts = cellValue.ToString().Split(',');
            foreach (var part in parts)
            {
                var subParts = part.Split('@');
                var axisName = subParts[1];
                var memberName = subParts[0];
                var explicitMember = new ExplicitMember(ToXName(axisName, namespaceDeclarationsElement), ToXName(memberName, namespaceDeclarationsElement));
                fact.ExplicitMembers.Add(explicitMember);
            }
        }

        private XName ToXName(string colonSeparatedName, XElement namespaceDeclarationsElement)
        {
            var parts = colonSeparatedName.Split(':');
            var prefix = parts[0];

            var xNamespace = namespaceDeclarationsElement.GetNamespaceOfPrefix(prefix);
            if (xNamespace == null)
            {
                throw new InvalidOperationException($"The prefix {prefix} is not in list of namespace declarations.");
            }

            return xNamespace + parts[1];
        }
    }
}