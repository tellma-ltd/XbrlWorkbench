using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class DefinitionLink : Link
    {
        public DefinitionLink(XName elementName, Uri role) : base(elementName, role)
        {
        }

        public override Uri ReferenceRole => LinkbaseRefRoles.Definition;

    }
}