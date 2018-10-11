using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class LinkbaseXNames
    {
        public static XName Linkbase = Namespaces.Link + "linkbase";

        public static XName SchemaLocation = Namespaces.Xsi + "schemaLocation";
        public static string SchemaLocationValue = "http://www.xbrl.org/2003/linkbase http://www.xbrl.org/2003/xbrl-linkbase-2003-12-31.xsd";

        public static XName Label = Namespaces.Link + "labelLink";
        public static XName Presentation = Namespaces.Link + "presentationLink";
        public static XName Definition = Namespaces.Link + "definitionLink";
        public static XName Reference = Namespaces.Link + "referenceLink";
        public static XName Calculation = Namespaces.Link + "calculationLink";
    }
}