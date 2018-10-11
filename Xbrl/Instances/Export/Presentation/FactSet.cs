using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public class FactSet
    {
        /// <summary>
        ///     Each fact set remembers all slice operations that lead to the set.
        ///     This information is needed to determine the single cell fact.
        /// </summary>
        private readonly Stack<Member> _appliedSlices;

        private FactSet(ISet<FactModel> factModels, Stack<Member> appliedSlices)
        {
            FactModels = factModels;
            _appliedSlices = appliedSlices;
        }


        public FactSet(IEnumerable<Fact> facts)
        {
            _appliedSlices = new Stack<Member>();

            FactModels = new HashSet<FactModel>();

            foreach (var fact in facts)
            {
                var factModel = new FactModel(fact);
                FactModels.Add(factModel);
            }
        }

        internal ISet<FactModel> FactModels { get; }

        /// <summary>
        ///     Returns a new fact set that contains only the facts that match a given member.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="includeRelatedAspects">
        /// </param>
        public FactSet Slice(Member member, bool includeRelatedAspects)
        {
            Func<FactModel, ISet<XName>> factDimensionNames = fm => new HashSet<XName>(fm.Aspects.OfType<ExplicitMemberAspect>().Select(a => a.Dimension.Name));
            Func<FactModel, bool> hypercubeDimensionsMatch = fm => member.HypercubeDimensionNames==null || member.HypercubeDimensionNames.SetEquals(factDimensionNames(fm));
            Func<FactModel, bool> containsAspect = fm => fm.Aspects.Contains(member.Aspect);
            Func<FactModel, bool> containsRelatedAspect = fm => includeRelatedAspects && member.RelatedAspects.Select(ra => ra.Aspect).Any(a => fm.Aspects.Contains(a));
            Func<FactModel, bool> predicate = fm => hypercubeDimensionsMatch(fm) && (containsAspect(fm) || containsRelatedAspect(fm));

            var query = FactModels.Where(predicate);
            var matchingFactModels = new HashSet<FactModel>(query);

            var newAppliedSlices = new Stack<Member>(_appliedSlices);
            newAppliedSlices.Push(member);
            return new FactSet(matchingFactModels, newAppliedSlices);
        }


        /// <summary>
        ///     Returns a new fact set with only the facts that match the members of the given axis.
        ///     We use this operation with presentation networks and hypercube axes.
        /// </summary>
        /// <remarks>
        /// Reducing by a dimension seems sufficient and would be much faster.
        /// </remarks>
        public FactSet Reduce(Axis axis)
        {
            var allMembers = axis.Roots.Flatten(m => m.Children).ToList();
            var mergedFactModels = new HashSet<FactModel>();

            foreach (var member in allMembers)
            {
                var slicedFactSet = Slice(member, includeRelatedAspects: false);
                mergedFactModels.UnionWith(slicedFactSet.FactModels);
            }

            return new FactSet(mergedFactModels, _appliedSlices);
        }


        /// <summary>
        /// Returns the fact in the location of the applied slices. Because of related aspects in members,
        /// the fact set can contain multiple candidates. The applied slices determine which candidate (if any) wins.
        /// </summary>
        public FactModel GetCellFact()
        {
            var remainingFactsCount = FactModels.Count;
            if (remainingFactsCount == 0)
            {
                return null;
            }

            var appliedConceptSlice = (ConceptMember)_appliedSlices.Single(s => s.Aspect is ConceptAspect) ;
            if (appliedConceptSlice.Item.PeriodType == PeriodTypes.Duration)
            {
                if (remainingFactsCount>1)
                {
                    throw new InvalidOperationException("There should be a unique cell fact for duration concepts.");
                }

                return FactModels.Single();
            }

            var appliedPeriodSlice = _appliedSlices.Single(s => s.Aspect is PeriodAspect);
            var periodAspect = (PeriodAspect)appliedPeriodSlice.Aspect;
            if (periodAspect.Period.PeriodType==PeriodTypes.Instant)
            {
                if (remainingFactsCount > 1)
                {
                    throw new InvalidOperationException("There should be a unique cell fact when slices by an instant period.");
                }

                return FactModels.Single();
            }

            // We reached the interesting case when slicing by an instant concept and a duration period.
            // This can result in up to two facts, and the preferred label role determines which one to pick.

            var requestedRelatedAspectRole = appliedConceptSlice.PreferredLabelRole == LabelRoles.PeriodStart ? RelatedAspectRoles.PeriodStart : RelatedAspectRoles.PeriodEnd;
            var requestedRelatedAspect = appliedPeriodSlice.RelatedAspects.SingleOrDefault(ra => ra.Role == requestedRelatedAspectRole);
            if (requestedRelatedAspect==null)
            {
                // Request cannot be satisfied, sorry.
                return null;
            }

            var matchingFact = FactModels.Where(fm => fm.Aspects.Contains(requestedRelatedAspect.Aspect)).ToList();
            if (!matchingFact.Any())
            {
                // No matching fact for the requested role, sorry.
                return null;
            }

            if (matchingFact.Count > 1)
            {
                throw new InvalidOperationException($"{matchingFact.Count} facts found for the related aspect role {requestedRelatedAspectRole}.");
            }

            return matchingFact.Single();
        }

        /// <summary>
        ///     Ensures absence of collisions.
        /// </summary>
        internal void EnsureNoCollisions()
        {
            var duplicates = FactModels
                .GroupBy(x => x.GetAspectsHashString())
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicates != null)
            {
                throw new InvalidOperationException("The fact set contains facts models with identical aspect sets. Such so-called collisions are not allowed.");
            }

            foreach (var factModel in FactModels)
            {
                factModel.EnsureDistinctDimensions();
            }
        }

        public void EnsureStandardAxesContainFactAspects(IList<Axis> standardAxes)
        {
            var entityAxis = FindAxis(standardAxes, Dimension.EntityDimension.Name);
            EnsureStandardAxisContainFactAspects<EntityAspect>(entityAxis);

            var unitAxis = FindAxis(standardAxes, Dimension.UnitDimension.Name);
            EnsureStandardAxisContainFactAspects<UnitAspect>(unitAxis);

            var periodAxis = FindAxis(standardAxes, Dimension.PeriodDimension.Name);
            EnsureStandardAxisContainFactAspects<PeriodAspect>(periodAxis);
        }

        private void EnsureStandardAxisContainFactAspects<TAspect>(Axis standardAxis) where TAspect:Aspect
        {
            var factAspects = new HashSet<Aspect>(FactModels.SelectMany(fm => fm.Aspects.OfType<TAspect>()));
            var axisAspects = new HashSet<Aspect>(standardAxis.Linearise().Select(m => m.Aspect));
            axisAspects.Add(new UnitAspect(TextUnit.Instance)); // Add the virtual text unit aspect.

            factAspects.ExceptWith(axisAspects);

            if (factAspects.Any())
            {
                var orphanedAspects = string.Join(", ", factAspects.Select(a => a.ToString(CultureInfo.InvariantCulture)));
                throw new InvalidOperationException($"The standard axis {standardAxis.Name} misses the following fact aspects: {orphanedAspects}.");
            }
        }

        private Axis FindAxis(IList<Axis> axes, XName name)
        {
            return axes.Single(a => a.Name == name);
        }

    }
}