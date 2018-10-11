using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    internal static class HtmlXNames
    {
        private static readonly XNamespace XhtmlNs = "http://www.w3.org/1999/xhtml";

        // Elements
        public static XName Html = XhtmlNs + "html";
        public static XName Head = XhtmlNs + "head";
        public static XName Body = XhtmlNs + "body";
        public static XName Div = XhtmlNs + "div";
        public static XName Span = XhtmlNs + "span";
        public static XName Article = XhtmlNs + "article";
        public static XName Header = XhtmlNs + "header";
        public static XName H1 = XhtmlNs + "h1";
        public static XName H2 = XhtmlNs + "h2";

        public static XName Table = XhtmlNs + "table";
        public static XName Colgroup = XhtmlNs + "colgroup";
        public static XName Col = XhtmlNs + "col";
        public static XName Thead = XhtmlNs + "thead";
        public static XName Tbody = XhtmlNs + "tbody";
        public static XName Tr = XhtmlNs + "tr";
        public static XName Th = XhtmlNs + "th";
        public static XName Td = XhtmlNs + "td";

        public static XName Script = XhtmlNs + "script";
        public static XName Link = XhtmlNs + "link";


        // Attributes
        public static XName Title = XhtmlNs + "title";
        public static XName Id = XhtmlNs + "id";
    }
}