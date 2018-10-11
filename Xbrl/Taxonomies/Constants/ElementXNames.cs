using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class ElementXNames
    {
        public static XName Element = Namespaces.Xsd + "element";

        public static string Abstract = "abstract";
        public static string Id = "id";
        public static string Name = "name";
        public static string Nillable = "nillable";
        public static string SubstitutionGroup = "substitutionGroup";
        public static string Type = "type";
        public static XName PeriodType = Namespaces.Xbrli + "periodType";
        public static XName Balance = Namespaces.Xbrli + "balance";
    }
}