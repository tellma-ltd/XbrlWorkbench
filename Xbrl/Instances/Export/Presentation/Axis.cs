using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <summary>
    /// An axis arranges the members of a given dimension as a tree structure. The presenter chains axes to present a fact set visually.
    /// The presenter builds up an axis for the standard dimensions Entity, Period and Units according to the order
    /// defined in the XBRL instance, plus an axis for each presentation network and for each hypercube dimension.
    /// </summary>
    public class Axis
    {
        /// <summary>
        /// The same name as the dimension name for all but presentation networks,
        /// where the axis name is the presentation role, while the name of the dimension is "ConceptDimension".
        /// </summary>
        public XName Name { get; }

        public Dimension Dimension { get; }

        public IList<Member> Roots { get; }

        public Axis(Dimension dimension, XName name, IEnumerable<Member> rootMembers)
        {
            Dimension = dimension;
            Name = name;
            Roots = rootMembers.ToList();
        }

        /// <summary>
        /// Reduces the axis by keeping only the members with aspects that actually exist in the fact set.
        /// The structure remains a proper, trimmed-down tree, that means parents remain if a descendant remains.
        /// </summary>
        public Axis Reduce(FactSet facts)
        {
            var reducedRoots = Roots.Select(m => m.Reduce(facts, null)).Where(m => m != null).ToList();
            return new Axis(Dimension, Name, reducedRoots);
        }

        /// <summary>
        /// Returns the flattened list of all members, with the parents before the children.
        /// </summary>
        /// <returns></returns>
        internal IList<Member> Linearise()
        {
            return Roots.Flatten(m => m.Children).ToList();
        }

        public Axis CreateChildAxis(Member rootMember)
        {
            return new Axis(Dimension, Name, new [] { rootMember });
        }
    }
}