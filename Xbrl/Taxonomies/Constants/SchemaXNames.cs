using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class SchemaXNames
    {
        public static XName Schema = Namespaces.Xsd + "schema";
        public static string TargetNamespace = "targetNamespace";

        public static XName Annotation = Namespaces.Xsd + "annotation";
        public static XName AppInfo = Namespaces.Xsd + "appinfo";
    }

    public static class ImportXNames
    {
        public static XName Import = Namespaces.Xsd + "import";
        public static string Namespace = "namespace";
        public static string SchemaLocation = "schemaLocation";
    }

}