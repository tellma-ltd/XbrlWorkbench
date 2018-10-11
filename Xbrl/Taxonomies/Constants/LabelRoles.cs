using System;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    /// <summary>
    /// The complete gallery of all known label roles.
    /// </summary>
    /// <remarks>
    /// - Note that the year differs. XBRL seems to roll out new roles from time to time.
    /// - Negative seems to have been superseded by negated in 2009. (See https://specifications.xbrl.org/registries/lrr-2.0/)
    /// - IFRS taxonomies only use a tiny subset of these roles, as listed in the architecture document.
    /// </remarks>
    public static class LabelRoles
    {
        public static Uri Standard = new Uri("http://www.xbrl.org/2003/role/label");

        public static Uri Total = new Uri("http://www.xbrl.org/2003/role/totalLabel");
        public static Uri Net = new Uri("http://www.xbrl.org/2009/role/netLabel");
        public static Uri[] Totals = { Total, Net };

        public static Uri Terse = new Uri("http://www.xbrl.org/2003/role/terseLabel");
        public static Uri Verbose = new Uri("http://www.xbrl.org/2003/role/verboseLabel");

        public static Uri Positive = new Uri("http://www.xbrl.org/2003/role/positiveLabel");
        public static Uri PositiveTerse = new Uri("http://www.xbrl.org/2003/role/positiveTerseLabel");
        public static Uri PositiveVerbose = new Uri("http://www.xbrl.org/2003/role/positiveVerboseLabel");

        public static Uri Negative = new Uri("http://www.xbrl.org/2003/role/negativeLabel");
        public static Uri NegativeTerse = new Uri("http://www.xbrl.org/2003/role/negativeTerseLabel");
        public static Uri NegativeVerbose = new Uri("http://www.xbrl.org/2003/role/negativeVerboseLabel");
        public static Uri Negated = new Uri("http://www.xbrl.org/2009/role/negatedLabel");
        public static Uri NegatedTerse = new Uri("http://www.xbrl.org/2009/role/negatedTerseLabel");
        public static Uri NegatedTotal = new Uri("http://www.xbrl.org/2009/role/negatedTotalLabel");
        public static Uri NegatedNet = new Uri("http://www.xbrl.org/2009/role/negatedNetLabel");
        public static Uri[] Negatives = { Negative, NegativeTerse, NegativeVerbose, Negated, NegatedTerse, NegatedTotal, NegatedNet };

        public static Uri Zero = new Uri("http://www.xbrl.org/2003/role/zeroLabel");
        public static Uri ZeroTerse = new Uri("http://www.xbrl.org/2003/role/zeroTerseLabel");
        public static Uri ZeroVerbose = new Uri("http://www.xbrl.org/2003/role/zeroVerboseLabel");

        public static Uri PeriodStart = new Uri("http://www.xbrl.org/2003/role/periodStartLabel");
        public static Uri PeriodEnd = new Uri("http://www.xbrl.org/2003/role/periodEndLabel");

        public static Uri Deprecated = new Uri("http://www.xbrl.org/2003/role/deprecatedLabel");
        public static Uri DeprecatedDate = new Uri("http://www.xbrl.org/2003/role/deprecatedDateLabel");


        public static Uri Documentation = new Uri("http://www.xbrl.org/2003/role/documentation"); // Careful: the IFRS architecture documentation writes 2008, which is wrong.
        public static Uri DocumentationGuidance = new Uri("http://www.xbrl.org/2003/role/definitionGuidance");
        public static Uri DisclosureGuidance = new Uri("http://www.xbrl.org/2003/role/disclosureGuidance");
        public static Uri PresentationGuidance = new Uri("http://www.xbrl.org/2003/role/presentationGuidance");
        public static Uri MeasurementGuidance = new Uri("http://www.xbrl.org/2003/role/measurementGuidance");
        public static Uri CommentaryGuidance = new Uri("http://www.xbrl.org/2003/role/commentaryGuidance");
        public static Uri ExampleGuidance = new Uri("http://www.xbrl.org/2003/role/exampleGuidance");
    }
}