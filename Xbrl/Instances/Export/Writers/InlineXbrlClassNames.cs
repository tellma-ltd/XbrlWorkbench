namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public static class InlineXbrlClassNames
    {
        public static string Network = "ixbrl-network";

        public static string Member = "ixbrl-member";
        public static string ItemMember = "ixbrl-member-item";
        public static string AbstractMember = "ixbrl-member-abstract";
        public static string TotalMember = "ixbrl-member-total";

        public static string Fact = "ixbrl-fact";
        public static string NonFractionFact = "ixbrl-fact-nonfraction";
        public static string NonNumericFact = "ixbrl-fact-nonnumeric";

        public static string PositiveFact = "ixbrl-fact-positive";

        /// <summary>
        /// This applies to display only. The underlying fact value can be positive or negative
        /// independent of its display. This is governed by the label role.
        /// </summary>
        public static string NegativeFact = "ixbrl-fact-negative";

        public static string Level = "ixbrl-level-{0}";
        public static string Table = "ixbrl-table";
    }
}