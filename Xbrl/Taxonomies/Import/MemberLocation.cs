using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class MemberLocation : ItemLocation
    {
        /// <summary>
        /// The same dimension name is used is multiple hypercubes, that is why the dimension name needs to be specified.
        /// </summary>
        /// <example>
        /// {http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}SegmentsAxis exists in the three hypercubes 
        /// {http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}DisclosureOfImpairmentLossRecognisedOrReversedTable 
        /// {http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}DisclosureOfDisaggregationOfRevenueFromContractsWithCustomersTable 
        /// {http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}DisclosureOfOperatingSegmentsTable 
        /// </example>
        public XName HypercubeName { get; set; }

        public XName DimensionName { get; set; }
    }
}