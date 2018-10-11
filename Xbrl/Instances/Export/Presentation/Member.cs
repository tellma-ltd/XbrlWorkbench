using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <summary>
    ///     A node in a Dimension. Members build a hierarchy, which is used for concept trees, line items and axes.
    ///     Specialised members can inherit from this class to add extra information, namely concept and axis members,
    ///     which both add label information.
    /// </summary>
    public class Member
    {
        public Member(Aspect aspect)
        {
            Aspect = aspect;
            RelatedAspects = new List<RelatedAspect>();
            Children = new List<Member>();
        }

        public Member Parent { get; set; }

        public IList<Member> Children { get; private set; }


        /// <summary>
        ///     The aspect represented by the member. Note that the same aspect can appear in multiple members
        ///     (from different presentation networks).
        /// </summary>
        public Aspect Aspect { get; }

        public IList<RelatedAspect> RelatedAspects { get; }

        /// <summary>
        /// For concept members, it is critical that the set of hypercube dimensions match *exactly*
        /// with the facts during a slice operation, otherwise erroneous facts remain in the sliced set.
        /// </summary>
        /// <remarks>
        /// Members other than concept members have this property set to null.
        /// </remarks>
        public ISet<XName> HypercubeDimensionNames { get; set; }

        /// <summary>
        /// Reduces the sub tree by keeping only the members with aspects that actually exist in the fact set.
        /// The structure remains a proper, trimmed-down tree, that means parents remain if they have used members.
        /// </summary>
        /// <returns>
        /// The cloned and trimmed sub tree or null of the entire sub tree disappeared.
        /// </returns>
        /// <remarks>
        /// Alternatively, one could use the FactSet.Slice method to decide if the member stays. This avoids the
        /// duplicated predicates but is very wasteful: it calculates the resulting sliced facts sets only for a yes/no decision.
        /// The ideal approach seems to be to isolate the predicate itself for use in both in this method and in FactSet.Slice.
        /// </remarks>
        internal Member Reduce(FactSet facts, Member newParent)
        {
            Func<FactModel, ISet<XName>> factDimensionNames = fm => new HashSet<XName>(fm.Aspects.OfType<ExplicitMemberAspect>().Select(a => a.Dimension.Name));
            Func<FactModel, bool> hypercubeDimensionsMatch = fm => HypercubeDimensionNames == null || HypercubeDimensionNames.SetEquals(factDimensionNames(fm));
            Func<FactModel, bool> containsAspect = fm => fm.Aspects.Contains(Aspect);

            var clone = Clone(); // Slightly wasteful to clone before knowing if the clone even survives. No biggie though.
            clone.Parent = newParent;
            clone.Children = Children.Select(ch => ch.Reduce(facts, clone)).Where(m => m != null).ToList();
            return clone.Children.Any() || facts.FactModels.Any(fm => containsAspect(fm) && hypercubeDimensionsMatch(fm)) ? clone : null;
        }

        public virtual string ToString(IFormatProvider formatProvider)
        {
            return Aspect.ToString(formatProvider);
        }

        /// <summary>
        /// The depth in the tree. Root members have depth 1.
        /// </summary>
        public int Depth
        {
            get
            {
                var level = 1;
                var iMember = this;
                while (iMember.Parent != null)
                {
                    iMember = iMember.Parent;
                    level += 1;
                }

                return level;
            }
        }

        internal Member Clone()
        {
            return (Member)MemberwiseClone();
        }

        public void AddChild(Member child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}