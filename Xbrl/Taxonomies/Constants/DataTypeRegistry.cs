namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class DataTypeRegistry
    {
        public static string Monetary = "xbrli:monetaryItemType";
        public static string String = "xbrli:stringItemType";
        public static string TextBlock = "nonnum:textBlockItemType";
        public static string DomainItem = "nonnum:domainItemType";
        public static string Percent = "num:percentItemType";
        public static string PerShare = "num:perShareItemType";

        public static string[] Textual = {String, TextBlock};

    }
}