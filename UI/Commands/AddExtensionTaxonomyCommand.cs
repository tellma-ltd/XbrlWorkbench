using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Taxonomies.Constants;
using Banan.Tools.Xbrl.Taxonomies.Import;
using Unity.Attributes;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Add-ExtensionTaxonomy", CommandType = typeof(AddExtensionTaxonomyCommand))]
    public class AddExtensionTaxonomyCommand : ShellCommandBase
    {
        [Dependency]
        public Instance Instance { get; set; }


        public override void Invoke()
        {
            base.Invoke();

            XNamespace ifrsNamespace = "http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full";
            XNamespace bSharpNamespace = "http://banan-it.com/taxonomy/2018-07-05/bsharp";

            var extension = new TaxonomyExtension("banan", bSharpNamespace);
            var concept1 = new ExtensionConcept("ComputerEquipment", DataTypeRegistry.Monetary, BalanceTypes.Debit, PeriodTypes.Instant);

            var notesSubclassificationsOfAssetsLiabilitiesAndEquitiesRole = new Uri("http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-800100");
            concept1.AddLocation(
                notesSubclassificationsOfAssetsLiabilitiesAndEquitiesRole,
                ifrsNamespace + "PropertyPlantAndEquipmentAbstract", ifrsNamespace + "OfficeEquipment");
            concept1.AddLabel("Computer equipment", "en");
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

            Instance.Dts.AddTaxonomy(new Uri("banan-20180710.xsd", UriKind.Relative), extension);
        }

    }
}