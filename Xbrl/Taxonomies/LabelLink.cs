using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class LabelLink : Link
    {
        public LabelLink(XName elementName, Uri role) : base(elementName, role)
        {
        }

        public override Uri ReferenceRole => LinkbaseRefRoles.Label;

    }
}