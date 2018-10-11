using System;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// The base class for arcs between two concepts.
    /// </summary>
    public abstract class InterConceptArc : Arc
    {
        protected InterConceptArc(XElement xArc) : base(xArc)
        {
        }

        protected InterConceptArc(Uri role, LocatorNode from, LocatorNode to) : base(role, from, to)
        {
        }

        public LocatorNode FromLocator => From as LocatorNode;

        public LocatorNode ToLocator => To as LocatorNode;
    }
}