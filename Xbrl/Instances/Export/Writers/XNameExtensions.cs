using System;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public static class XNameExtensions
    {
        public static string ToColonSeparated(this XName name, XElement namespaceDeclarationsElement)
        {
            var prefix = namespaceDeclarationsElement.GetPrefixOfNamespace(name.Namespace);
            if (prefix==null)
            {
                throw new InvalidOperationException($"Could not convert {name} to a colon-separated format because the namespace is not declared.");
            }

            return $"{prefix}:{name.LocalName}";
        }
    }
}