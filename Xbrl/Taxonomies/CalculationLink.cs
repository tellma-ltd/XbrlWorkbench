using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class CalculationLink : Link
    {
        public CalculationLink(XName elementName, Uri role) : base(elementName, role)
        {
        }
        public override Uri ReferenceRole => LinkbaseRefRoles.Calculation;

    }
}