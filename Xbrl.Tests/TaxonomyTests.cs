using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.Xbrl.Taxonomies.Constants;
using Banan.Tools.Xbrl.Taxonomies.Export.Writers;
using Banan.Tools.Xbrl.Taxonomies.Import;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Banan.Tools.Xbrl.Tests
{
    [TestClass]
    public class TaxonomyTests
    {
        [TestMethod]
        public void AddFromFileSet()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            // Act
            var dts = new DiscoverableTaxonomySet();
            var taxonomy = dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);
        }

        [TestMethod]
        public void AddFromExtension()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var dts = new DiscoverableTaxonomySet();
            var baseTaxonomy = dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);

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

            // Act
            var extensionTaxonomy = dts.AddTaxonomy(new Uri("banan-20180710.xsd", UriKind.Relative), extension);
        }

        [TestMethod]
        public void WriteBaseTaxonomy()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var dts = new DiscoverableTaxonomySet();
            var baseTaxonomy = dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);

            // Act
            var writer = new ZipArchiveWriter();
            baseTaxonomy.Save(writer);

            // Assert
            File.WriteAllBytes(@"C:\temp\IFRST_2017-03-09-exported.zip", writer.ZipArchiveBytes);
        }

        [TestMethod]
        public void WriteExtensionTaxonomy()
        {
            // Arrange
            var zipArchive = ZipFile.Open(@"Samples\IFRST_2017-03-09.zip", ZipArchiveMode.Read);
            var fileReader = new ZipArchiveReader(zipArchive);
            var taxonomySource = new TaxonomyFileSet(fileReader);

            var dts = new DiscoverableTaxonomySet();
            var baseTaxonomy = dts.AddTaxonomy(new Uri("http://xbrl.ifrs.org/taxonomy/2017-03-09/full_ifrs_entry_point_2017-03-09.xsd"), taxonomySource);

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

            var extensionTaxonomy = dts.AddTaxonomy(new Uri("banan-20180710.xsd", UriKind.Relative), extension);

            // Act
            var writer = new ZipArchiveWriter();
            extensionTaxonomy.Save(writer);

            // Assert
            File.WriteAllBytes(@"C:\temp\banan-20180710.zip", writer.ZipArchiveBytes);
        }
    }
}