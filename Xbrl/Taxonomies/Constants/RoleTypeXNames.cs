using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class RoleTypeXNames
    {
        public static XName RoleType = Namespaces.Link + "roleType";

        public static string Id = "id";
        public static string Role = "roleURI";

        public static XName Definition = Namespaces.Link + "definition";
        public static XName UsedOn = Namespaces.Link + "usedOn";
    }

    public static class RoleRefXNames
    {
        public static XName RoleRef = Namespaces.Link + "roleRef";

        public static string Role = "roleURI";

    }

}