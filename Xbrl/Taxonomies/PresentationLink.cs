using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class PresentationLink : Link
    {
        public PresentationLink(XName elementName, Uri role) : base(elementName, role)
        {
        }

        public IList<LocatorNode> GetRootNodes()
        {
            return NodesMap.Values
                .Where(n => !n.ToArcs.OfType<PresentationArc>().Any())
                .Cast<LocatorNode>()
                .ToList();
        }

        public override Uri ReferenceRole => LinkbaseRefRoles.Presentation;
    }
}