using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Instances.Export.Presentation;
using Banan.Tools.Xbrl.Instances.Export.Writers;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.Xbrl.Taxonomies.Constants;
using Banan.Tools.Xbrl.Taxonomies.Import;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using ExplicitMember = Banan.Tools.Xbrl.Instances.ExplicitMember;

namespace Banan.Tools.Xbrl.Tests
{
    [TestClass]
    public class InstanceTests
    {
        [TestMethod]
        public void WriteAsInlineXbrl()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var instance = LoadInstanceFromExcelDocument(@"Samples\BSharpIfrsFactsValuesSampleV1.xlsx");

            instance.Dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_doc_entry_point_2017-03-09.xsd"), taxonomySource);
            AddExtensionTaxonomy(instance.Dts);

            var writerSettings = new InlineXbrlWriterSettings
            {
                Title = "iXBRL Export",
                Culture = new CultureInfo("en-GB"),
                Scale = 3,
                Scope = new ScopeSettings
                {
                    PresentationNetworks = GetSupportedPresentationNetworks()
                },
                IncludeDocumentation = true,
                IncludeReferences = true
            };

            // Act
            var xTemplate = XDocument.Load(@"Samples\iXBRL.xhtml");
            var writer = new InlineXbrlWriter(xTemplate, writerSettings);
            instance.Save(writer);

            // Assert
            File.WriteAllText(@"C:\temp\banan-20180710.xhtml", writer.Document.ToString());
        }

        [TestMethod]
        public void WriteAsInlineXbrlFiling()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var instance = LoadInstanceFromExcelDocument(@"Samples\BSharpIfrsFactsValuesSampleV1.xlsx");
            instance.Dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);
            AddExtensionTaxonomy(instance.Dts);

            var writerSettings = new InlineXbrlFilingWriterSettings
            {
                Title = "iXBRL Export",
                Culture = new CultureInfo("en-GB"),
                Scale = 3,
                Scope = new ScopeSettings
                {
                    PresentationNetworks = GetSupportedPresentationNetworks()
                },
                InlineXbrlFileName = "banan-20180710.xhtml"
            };

            // Act
            var xTemplate = XDocument.Load(@"Samples\iXBRL.xhtml");
            var writer = new InlineXbrlFilingWriter(xTemplate, writerSettings);
            instance.Save(writer);

            // Assert
            File.WriteAllBytes(@"C:\temp\banan-20180710.zip", writer.ZipArchiveBytes);
        }

        [TestMethod]
        public void WriteAsXbrl()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var instance = LoadInstanceFromExcelDocument(@"Samples\BSharpIfrsFactsValuesSampleV1.xlsx");
            instance.Dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);
            AddExtensionTaxonomy(instance.Dts);

            // Act
            var writer = new XbrlWriter();
            instance.Save(writer);

            // Assert
            File.WriteAllText(@"C:\temp\banan-20180710.xml", writer.Document.ToString());
        }


        [TestMethod]
        public void WriteAsWord()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var instance = LoadInstanceFromExcelDocument(@"Samples\BSharpIfrsFactsValuesSampleV1.xlsx");
            instance.Dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);
            AddExtensionTaxonomy(instance.Dts);

            var writerSettings = new WordWriterSettings
            {
                Culture = new CultureInfo("en-GB"),
                Scale = 3,
                Scope = new ScopeSettings
                {
                    PresentationNetworks = GetSupportedPresentationNetworks()
                },
            };

            // Act
            var templateBytes = File.ReadAllBytes(@"Samples\Word.docx");
            var writer = new WordWriter(templateBytes, writerSettings);
            instance.Save(writer);

            // Assert
            File.WriteAllBytes(@"C:\temp\banan-20180710.docx", writer.DocumentBytes);
        }



        private void AddExtensionTaxonomy(DiscoverableTaxonomySet dts)
        {
            XNamespace bSharpNamespace = "http://banan-it.com/taxonomy/2018-07-05/bsharp";
            XNamespace ifrsNamespace = "http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full";

            var extension = new TaxonomyExtension("banan", bSharpNamespace);

            var concept1 = new ExtensionConcept("ComputerEquipment", DataTypeRegistry.Monetary, BalanceTypes.Debit, PeriodTypes.Instant);
            concept1.AddLabel("Computer equipment", "en");
            concept1.AddLocation(new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800100"), ifrsNamespace + "PropertyPlantAndEquipmentAbstract", ifrsNamespace + "OfficeEquipment");
            extension.ExtensionItems.Add(concept1);

            var member1 = new ExtensionMember("BestPaintMember");
            member1.AddLabel("Best Paint", "en");
            member1.AddLocation(
                ifrsNamespace + "DisclosureOfOperatingSegmentsTable", ifrsNamespace + "SegmentsAxis",
                ifrsNamespace + "ReportableSegmentsMember", null);
            extension.ExtensionItems.Add(member1);

            var member2 = new ExtensionMember("BestPlasticMember");
            member2.AddLabel("Best Plastic", "en");
            member2.AddLocation(
                ifrsNamespace + "DisclosureOfOperatingSegmentsTable", ifrsNamespace + "SegmentsAxis",
                ifrsNamespace + "ReportableSegmentsMember", bSharpNamespace + "BestPaintMember");
            extension.ExtensionItems.Add(member2);

            dts.AddTaxonomy(new Uri("banan-20180710.xsd", UriKind.Relative), extension);
        }

        private static PresentationNetwork[] GetSupportedPresentationNetworks()
        {
            var networks = new[]
            {
                new PresentationNetwork
                {
                    Name = "Statement of financial position, current/non-current",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-210000")
                },
                new PresentationNetwork
                {
                    Name = "Statement of comprehensive income, profit or loss, by function of expense",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-310000")
                },
                new PresentationNetwork
                {
                    Name = "Statement of cash flows, direct method",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_7_2017-03-09_role-510000")
                },
                new PresentationNetwork
                {
                    Name = "Statement of changes in equity",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-610000")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Subclassifications of assets, liabilities and equities",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800100")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Analysis of income and expense",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800200")
                },
                new PresentationNetwork
                {
                    Name = "Notes - List of notes",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800500")
                },
                new PresentationNetwork
                {
                    Name = "Notes - List of accounting policies",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800600")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Corporate information and statement of IFRS compliance",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-810000")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Property, plant and equipment",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_16_2017-03-09_role-822100")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Fair value measurement",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ifrs_13_2017-03-09_role-823000")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Investment property",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_40_2017-03-09_role-825100")
                },
                new PresentationNetwork
                {
                    Name = "Notes – Inventories",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_2_2017-03-09_role-826380")
                },
                new PresentationNetwork
                {
                    Name = "Notes – Revenue",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_18_2017-03-09_role-831110")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Revenue from contracts with customers",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ifrs_15_2017-03-09_role-831150")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Impairment of assets",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_36_2017-03-09_role-832410")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Employee benefits",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480")
                },
                new PresentationNetwork
                {
                    Name = "Notes - Operating segments",
                    Role = new Uri("http://xbrl.ifrs.org/role/ifrs/ifrs_8_2017-03-09_role-871100")
                },
            };
            return networks;
        }


        private Instance LoadInstanceFromExcelDocument(string fileName)
        {
            XNamespace bSharpNamespace = "http://banan-it.com/taxonomy/2018-07-05/bsharp";
            XNamespace ifrsNamespace = "http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full";

            var xDeclarationsHolder = new XElement("dummy",
                new XAttribute(XNamespace.Xmlns + "ifrs-full", ifrsNamespace),
                new XAttribute(XNamespace.Xmlns + "banan", bSharpNamespace));

            var instance = new Instance();

            var etbUnit = new CurrencyUnit("ETB");
            instance.Units.Add(etbUnit);

            var quarter1Of2018 = new DurationPeriod(new DateTime(2018, 1, 1), new DateTime(2018, 3, 31));
            var startOfQuarter1Of2018 = new InstantPeriod(new DateTime(2017, 12, 31));
            var endOfQuarter1Of2018 = new InstantPeriod(new DateTime(2018, 3, 31));
            instance.Periods.Add(quarter1Of2018);
            instance.Periods.Add(startOfQuarter1Of2018);
            instance.Periods.Add(endOfQuarter1Of2018);

            var wsi = new Entity
            {
                Id = "wsi",
                IdentifierScheme = new Uri("http://www.sec.gov/CIK"),
                Identifier = "WSI",
                Name = "Walia Steel Industry PLC"
            };
            instance.Entities.Add(wsi);

            var package = new ExcelPackage(new FileInfo(fileName));
            var worksheet = package.Workbook.Worksheets[1];

            ExcelRangeBase conceptCell = worksheet.Cells["B2"];
            ExcelRangeBase valueCell = worksheet.Cells["C2"];
            ExcelRangeBase startDateCell = worksheet.Cells["D2"];
            ExcelRangeBase dateOrEndDateCell = worksheet.Cells["E2"];
            ExcelRangeBase explicitMembersCell = worksheet.Cells["F2"];

            while (conceptCell.Value != null && valueCell.Value != null)
            {
                var concept = conceptCell.GetValue<string>();
                var value = valueCell.Value;
                var period = startDateCell.Value == null || string.IsNullOrEmpty(startDateCell.Value.ToString()) ?
                    new InstantPeriod(dateOrEndDateCell.GetValue<DateTime>()) :
                    (Period)new DurationPeriod(startDateCell.GetValue<DateTime>(), dateOrEndDateCell.GetValue<DateTime>());

                var conceptName = ToXName(concept, xDeclarationsHolder);
                var fact = new Fact(conceptName, wsi, period)
                {
                    Unit = etbUnit,
                    Value = value.ToString(),
                    BalanceMethod = BalanceMethod.Algebraic
                };

                AddExplicitMembers(fact, explicitMembersCell.Value, xDeclarationsHolder);

                instance.Facts.Add(fact);

                conceptCell = conceptCell.Offset(1, 0);
                valueCell = valueCell.Offset(1, 0);
                startDateCell = startDateCell.Offset(1, 0);
                dateOrEndDateCell = dateOrEndDateCell.Offset(1, 0);
                explicitMembersCell = explicitMembersCell.Offset(1, 0);
            }
            return instance;
        }

        private void AddExplicitMembers(Fact fact, object cellValue, XElement namespaceDeclarationsElement)
        {
            if (cellValue == null)
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