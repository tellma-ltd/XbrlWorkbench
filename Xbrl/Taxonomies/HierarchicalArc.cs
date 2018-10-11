using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// The base class for presentation and definition arcs.
    /// </summary>
    public abstract class HierarchicalArc : InterConceptArc
    {
        public double Order { get; }

        protected HierarchicalArc(XElement xArc) : base(xArc)
        {
            Order = double.Parse(xArc.Attribute(ArcXNames.Order).Value);
        }

        protected HierarchicalArc(Uri role, LocatorNode from, LocatorNode to, double order) : base(role, from, to)
        {
            Order = order;
        }

        protected override void Fill(XElement xNode)
        {
            base.Fill(xNode);
            xNode.Add(new XAttribute(ArcXNames.Order, Order));
        }
    }
}