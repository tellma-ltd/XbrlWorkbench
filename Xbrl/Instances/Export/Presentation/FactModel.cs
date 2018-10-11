using System;
using System.Collections.Generic;
using System.Linq;
using Banan.Tools.Xbrl.Taxonomies;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <summary>
    /// A FactModel wraps a Fact and exposes all aspects as a set of Aspects to enable cube operations.
    /// </summary>
    public class FactModel
    {
        internal readonly Fact Fact;

        /// <summary>
        /// The set of aspects give context to a fact value. The four standard XBRL aspects (entity, unit, period and concept)
        /// exist for all facts. (A dummy text unit is used for textual facts for normalisation.)
        /// </summary>
        public ISet<Aspect> Aspects { get; }

        public FactModel(Fact fact)
        {
            Fact = fact;
            Aspects = new HashSet<Aspect>();

            Aspects.Add(new EntityAspect(fact.Entity));
            Aspects.Add(new PeriodAspect(fact.Period));
            Aspects.Add(new ConceptAspect(fact.ConceptName));

            // Normalise to make all fact models have a unit aspect.
            var unit = fact.Unit ?? TextUnit.Instance;
            Aspects.Add(new UnitAspect(unit));

            foreach (var explicitMember in fact.ExplicitMembers)
            {
                var dimension = new Dimension(explicitMember.DimensionName);
                Aspects.Add(new ExplicitMemberAspect(dimension, explicitMember.MemberName));
            }
        }

        internal void EnsureDistinctDimensions()
        {
            var duplicates = Aspects
                .Select(a => a.Dimension)
                .GroupBy(x => x.Name)
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicates!=null)
            {
                throw new InvalidOperationException($"The dimension {duplicates.Key} appears {duplicates.Count()} times in the fact model.");
            }
        }

        internal void EnsureConsistentPeriodTypes(DiscoverableTaxonomySet dts)
        {
            var periodAspect = Aspects.OfType<PeriodAspect>().Single();
            var factPeriodType = periodAspect.Period.PeriodType;

            var conceptAspect = Aspects.OfType<ConceptAspect>().Single();
            var concept = dts.FindItem(conceptAspect.ConceptName);

            if (concept == null)
            {
                throw new InvalidOperationException($"The concept {conceptAspect.ConceptName} does not exist in the DTS. If this is a custom concept, then it needs to be defined in an extension taxonomy that is added to the DTS.");
            }

            var conceptPeriodType = concept.PeriodType;

            if (factPeriodType!=conceptPeriodType)
            {
                throw new InvalidOperationException($"The fact for the concept {concept.Name} has period type {factPeriodType} but should be {conceptPeriodType}.");
            }
        }

        /// <summary>
        /// Used internally to identify collisions.
        /// </summary>
        /// <returns></returns>
        internal string GetAspectsHashString()
        {
            var aspectsHashString = string.Join(",", Aspects.Select(a => a.GetHashCode().ToString()));
            return aspectsHashString;
        }
    }
}