using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class ArcXNames
    {
        public static XName Label = Namespaces.Link + "labelArc";
        public static XName Reference = Namespaces.Link + "referenceArc";
        public static XName Calculation = Namespaces.Link + "calculationArc";
        public static XName Definition = Namespaces.Link + "definitionArc";
        public static XName Presentation = Namespaces.Link + "presentationArc";

        public static string Weight = "weight";
        public static string Order = "order";
        public static string PreferredLabelRole = "preferredLabel";
    }
}