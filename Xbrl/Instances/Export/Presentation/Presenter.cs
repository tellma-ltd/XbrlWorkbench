using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public class Presenter
    {
        private ScopeSettings _settings;

        public Presenter(Instance instance)
        {
            Instance = instance;
        }

        public Instance Instance { get; }

        /// <summary>
        ///     Builds up the specified presentation networks by filling in the facts from the XBRL instance.
        ///     The format-specific rendering is done by the writer which assembles the final result.
        /// </summary>
        public void Present(IPresenterWriter presenterWriter, ScopeSettings settings)
        {
            _settings = settings;

            var factSet = new FactSet(Instance.Facts);


            factSet.EnsureNoCollisions();
            foreach (var factModel in factSet.FactModels)
            {
                factModel.EnsureDistinctDimensions();
                factModel.EnsureConsistentPeriodTypes(Instance.Dts);
            }

            var standardAxes = BuildStandardAxes();
            factSet.EnsureStandardAxesContainFactAspects(standardAxes);

            presenterWriter.WriteBeginExport(Instance);

            // 1. Slice by single entity.
            var entityAxis = FindAxis(standardAxes, Dimension.EntityDimension.Name);
            var reducedEntityAxis = entityAxis.Reduce(factSet);
            if (!reducedEntityAxis.Roots.Any())
            {
                throw new InstanceExportException("The reduced entity axis is empty. What the ?!");
            }

            if (reducedEntityAxis.Roots.Count > 1)
            {
                throw new InstanceExportException("The instance contains more than one entity. This is not supported.");
            }

            var entityMember = entityAxis.Roots[0];
            var entityAspect = (EntityAspect) entityMember.Aspect;
            factSet = factSet.Slice(entityMember, false);

            // 2. Slice by single currency unit.
            var unitAxis = FindAxis(standardAxes, Dimension.UnitDimension.Name);
            var reducedUnitAxis = unitAxis.Reduce(factSet);
            if (!reducedUnitAxis.Roots.Any())
            {
                throw new InstanceExportException("The reduced unit axis is empty. What the ?!");
            }

            var hasNonCurrencyAspects = reducedUnitAxis.Roots.Select(r => (UnitAspect) r.Aspect).Any(ua => !(ua.Unit is CurrencyUnit));
            if (hasNonCurrencyAspects)
            {
                throw new InstanceExportException("The instance contains non-currency units. This is not supported yet.");
            }

            var currencyMembers = reducedUnitAxis.Roots.Where(m => ((UnitAspect) m.Aspect).Unit is CurrencyUnit).ToList();
            if (currencyMembers.Count > 1)
            {
                throw new InstanceExportException("The instance contains more than one currency unit. This is not supported.");
            }

            var currencyMember = currencyMembers[0];
            var currencyAspect = (UnitAspect) currencyMember.Aspect;
            factSet = factSet.Slice(currencyMember, true); // textual facts will remain because the 'text unit' is related to all currency units.

            // 3. Determine first duration period.
            var periodAxis = FindAxis(standardAxes, Dimension.PeriodDimension.Name);
            var reducedPeriodAxis = periodAxis.Reduce(factSet);
            if (!reducedPeriodAxis.Roots.Any())
            {
                throw new InstanceExportException("The reduced period axis is empty. What the ?!");
            }

            //var firstDurationAspect = reducedPeriodAxis.Roots.Select(r => (PeriodAspect) r.Aspect).FirstOrDefault(ua => ua.Period is DurationPeriod);
            //if (firstDurationAspect == null)
            //{
            //    throw new XbrlExportException("The instance does not contain any duration period. This is not supported.");
            //}

            // Instance looks okay. Ready to write it out.
            presenterWriter.WriteIntro(entityAspect.Entity, currencyAspect.Unit as CurrencyUnit);

            WriteNetworks(standardAxes, factSet, settings.PresentationNetworks, presenterWriter);

            presenterWriter.WriteEndExport();
        }

        private void WriteNetworks(IList<Axis> standardAxes, FactSet factSet, IList<PresentationNetwork> presentationNetworks, IPresenterWriter presenterWriter)
        {
            var periodAxis = FindAxis(standardAxes, Dimension.PeriodDimension.Name);

            foreach (var presentationNetwork in presentationNetworks)
            {
                var presentationLink = Instance.Dts.FindPresentationLink(presentationNetwork.Role);
                if (presentationLink == null)
                {
                    throw new InstanceExportException($"There is no presentation network {presentationNetwork.Role} in the DTS.");
                }

                var presentationNetworkAxis = BuildPresentationAxis(presentationLink);

                // Only keep facts in that network.
                var reducedFactSet = factSet.Reduce(presentationNetworkAxis);
                if (!reducedFactSet.FactModels.Any())
                {
                    // No fact in this network: nothing to show.
                    continue;
                }

                presenterWriter.WriteBeginPresentationNetwork(presentationNetwork);
                WriteTable(presenterWriter, presentationNetworkAxis, periodAxis, reducedFactSet);
                presenterWriter.WriteEndPresentationNetwork();
            }
        }

        private void WriteTable(IPresenterWriter presenterWriter, Axis verticalAxis, Axis horizontalAxis, FactSet facts)
        {
            // Reduce both axes for a compact display, unless disabled.
            var reducedVerticalAxis = _settings.IncludeEmptyConcepts ? verticalAxis : verticalAxis.Reduce(facts);
            var reducedHorizontalAxis = _settings.IncludeEmptyExplicitMembers ? horizontalAxis : horizontalAxis.Reduce(facts);

            if (!reducedVerticalAxis.Roots.Any())
            {
                throw new InstanceExportException("The reduced vertical axis is empty. This should not happen because them facts must be living somewhere...");
            }

            if (!reducedHorizontalAxis.Roots.Any())
            {
                throw new InstanceExportException("The reduced horizontal axis is empty. This should not happen because them facts must be living somewhere...");
            }

            if (horizontalAxis.Dimension == Dimension.PeriodDimension)
            {
                reducedHorizontalAxis = NormalisePeriodAxis(reducedHorizontalAxis);
            }

            presenterWriter.WriteBeginTable(reducedHorizontalAxis);

            var allHorizontalAxisMembers = reducedHorizontalAxis.Linearise();
            foreach (var verticalAxisRoot in reducedVerticalAxis.Roots.Cast<ConceptMember>())
            {
                WriteConceptMember(presenterWriter, verticalAxisRoot, reducedVerticalAxis, allHorizontalAxisMembers, facts);
            }

            presenterWriter.WriteEndTable();
        }

        /// <summary>
        /// This method ensures that a period axis has only instant periods or only duration periods.
        /// If the initial axis contains both types, the method returns a new axis with only the duration periods inside,
        /// assuming that the instant periods are subsumed inside the remaining duration periods as related aspects.
        /// </summary>
        private Axis NormalisePeriodAxis(Axis periodAxis)
        {
            var usedPeriodTypes = periodAxis.Roots.Select(m => ((PeriodAspect) m.Aspect).Period.PeriodType).Distinct();
            if (usedPeriodTypes.Count() <= 1)
            {
                return periodAxis;
            }

            var newRoots = new List<Member>();
            foreach (var member in periodAxis.Roots)
            {
                var periodAspect = (PeriodAspect) member.Aspect;
                if (periodAspect.Period is DurationPeriod)
                {
                    newRoots.Add(member.Clone());
                }
            }

            return new Axis(periodAxis.Dimension, periodAxis.Name, newRoots);
        }

        private void WriteConceptMember(IPresenterWriter presenterWriter, ConceptMember conceptMember, Axis verticalAxis, IList<Member> horizontalAxisMembers, FactSet facts)
        {
            var hypercubeAxis = conceptMember.HypercubeAxis;
            if (hypercubeAxis != null)
            {
                if (horizontalAxisMembers.Count != 1)
                {
                    throw new InvalidOperationException($"Hypercubes require a single period, but the hypercube '{conceptMember.Label}' has {horizontalAxisMembers.Count} periods.");
                }

                // It is crucial to include related aspects to keep beginning/end of period
                // facts in the fact set.
                var periodSlicedFactSet = facts.Slice(horizontalAxisMembers.Single(), includeRelatedAspects: true);

                var reducedFactSet = periodSlicedFactSet.Reduce(hypercubeAxis);
                if (!reducedFactSet.FactModels.Any())
                {
                    // No fact in this hypercube axis: nothing to show.
                    // This should not happen since the concept axis got reduced already.
                    return;
                }

                var verticalChildAxis = verticalAxis.CreateChildAxis(conceptMember);

                // Clear the hypercube axis to signal consumption and avoid infinite loop.
                conceptMember.HypercubeAxis = null;

                WriteTable(presenterWriter, verticalChildAxis, hypercubeAxis, reducedFactSet);
                return;
            }

            var verticallySlicedFactSet = facts.Slice(conceptMember, false);
            var cellFacts = new Dictionary<Member, FactModel>();
            foreach (var horizontalAxisMember in horizontalAxisMembers)
            {
                // It is crucial to include related aspects to keep beginning/end of period
                // facts in the fact set.
                var horizontallySlicedFactSet = verticallySlicedFactSet.Slice(horizontalAxisMember, true);

                var cellFact = horizontallySlicedFactSet.GetCellFact();
                cellFacts[horizontalAxisMember] = cellFact;
            }

            presenterWriter.WriteConcept(conceptMember, cellFacts);

            foreach (var childMember in conceptMember.Children.Cast<ConceptMember>())
            {
                WriteConceptMember(presenterWriter, childMember, verticalAxis, horizontalAxisMembers, facts);
            }
        }

        /// <summary>
        /// A definition network attaches one (or at least one?) hypercube to a concept.
        /// </summary>
        /// <remarks>
        /// The role of some definition linkbases have a suffix (a, b, ...) added to the role of the presentation network.
        /// We have to disregard this suffix to reliably find the attached hypercubes.
        /// 
        /// For example, for the presentation network with the role http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480
        /// there exist these definition network roles, each defining a single hypercube:
        /// http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480
        /// http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480a
        /// http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480b
        /// http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480c
        /// http://xbrl.ifrs.org/role/ifrs/ias_19_2017-03-09_role-834480d
        /// (All bundled inside the def_ias_19_2017-03-09_role-834480.xml linkbase)
        /// 
        /// Clearly, the authors of the IFRS taxonomy wanted to have a single hypercube per definition link.
        /// Question: is that a requirement from XBRL Dimensions?
        /// </remarks>
        /// <remarks>
        /// A hypercube arc (all or notAll) connects to a *concept* inside a specific presentation network, which is not actually precise.
        /// Instead, it should attach to a *locator node*.
        /// The same issue appears with extensions of existing presentation, definition or calculation networks.
        /// </remarks>
        private Hypercube GetAttachedHypercube(LocatorNode node)
        {
            var item = node.Item;

            // Oddly, the role of some definition linkbase have a suffix (a, b, ...) added to the role of the presentation network.
            // We have to disregard this suffix to reliably find the attached hypercubes.
            var hypercubeArc = item.Nodes
                .SelectMany(n => n.FromArcs.OfType<DefinitionArc>())
                .SingleOrDefault(a => a.Link.Role.AbsoluteUri.StartsWith(node.Link.Role.AbsoluteUri) && a.Role == ArcRoles.PositiveHypercube);

            if (hypercubeArc == null)
            {
                return null;
            }

            var hypercube = new Hypercube(hypercubeArc.ToLocator.Item);

            var dimensionNodes = hypercubeArc.ToLocator.FromArcs.OfType<DefinitionArc>().Select(a => a.ToLocator);
            foreach (var dimensionNode in dimensionNodes)
            {
                var axis = BuildHypercubeAxis(dimensionNode);
                hypercube.Axes.Add(axis);
            }

            return hypercube;
        }

        private Axis FindAxis(IList<Axis> axes, XName name)
        {
            return axes.Single(a => a.Name == name);
        }

        private IList<Axis> BuildStandardAxes()
        {
            var axes = new List<Axis>();
            axes.Add(BuildEntityAxis(Instance.Entities));
            axes.Add(BuildUnitAxis(Instance.Units));
            axes.Add(BuildPeriodAxis(Instance.Periods));
            return axes;
        }

        private Axis BuildEntityAxis(IList<Entity> entities)
        {
            var dimension = Dimension.EntityDimension;
            var rootMembers = entities.Select(e => new Member(new EntityAspect(e)));
            return new Axis(dimension, dimension.Name, rootMembers);
        }

        private Axis BuildUnitAxis(IList<Unit> units)
        {
            var textUnitAspect = new UnitAspect(TextUnit.Instance);
            var dimension = Dimension.UnitDimension;
            var rootMembers = new List<Member>();
            foreach (var unit in units)
            {
                var member = new Member(new UnitAspect(unit));

                // The text unit aspect is a related aspect in all unit members.
                var relatedAspect = new RelatedAspect(textUnitAspect, RelatedAspectRoles.Text);
                member.RelatedAspects.Add(relatedAspect);

                rootMembers.Add(member);
            }

            return new Axis(dimension, dimension.Name, rootMembers);
        }

        private Axis BuildPeriodAxis(IList<Period> periods)
        {
            var periodAspects = periods.Select(e => new PeriodAspect(e)).ToList();
            var instantPeriodAspects = periodAspects.Where(pa => pa.Period is InstantPeriod).ToList();

            var rootMembers = new List<Member>();
            foreach (var periodAspect in periodAspects)
            {
                var member = new Member(periodAspect);
                if (periodAspect.Period.PeriodType == PeriodTypes.Duration)
                {
                    var durationPeriod = (DurationPeriod) periodAspect.Period;
                    AddRelatedInstantPeriod(member, durationPeriod.StartDate.AddDays(-1), instantPeriodAspects, RelatedAspectRoles.PeriodStart);
                    AddRelatedInstantPeriod(member, durationPeriod.EndDate, instantPeriodAspects, RelatedAspectRoles.PeriodEnd);
                }

                rootMembers.Add(member);
            }

            var dimension = Dimension.PeriodDimension;
            return new Axis(dimension, dimension.Name, rootMembers);
        }

        private void AddRelatedInstantPeriod(Member member, DateTime date, List<PeriodAspect> instantPeriodAspects, Uri role)
        {
            var aspect = instantPeriodAspects.SingleOrDefault(pa => ((InstantPeriod) pa.Period).Date == date);
            if (aspect != null)
            {
                var relatedAspect = new RelatedAspect(aspect, role);
                member.RelatedAspects.Add(relatedAspect);
            }
        }

        private Axis BuildPresentationAxis(PresentationLink presentationLink)
        {
            var rootNodes = presentationLink.GetRootNodes();

            // The IFRS taxonomy only has single roots as a workaround for the shortcoming of XBRL that the order of root nodes
            // is undefined.
            if (rootNodes.Count > 1)
            {
                throw new InvalidOperationException($"The presentation network {presentationLink.Role} has multiple root nodes. This should not be the case for IFRS taxonomies.");
            }

            var rootMembers = rootNodes.Select(n => TraversePresentationNetwork(n, null, new HashSet<XName>())).ToList();
            var axisNamespace = (XNamespace) presentationLink.Role.ToString();
            return new Axis(Dimension.ConceptDimension, axisNamespace + "Axis", rootMembers);
        }

        private ConceptMember TraversePresentationNetwork(LocatorNode node, Uri preferredLabelRole, ISet<XName> hypercubeDimensionNames)
        {
            var concept = node.Item;

            var aspect = new ConceptAspect(concept.Name);
            var member = new ConceptMember(aspect)
            {
                Item = concept,
                PreferredLabelRole = preferredLabelRole,
                Label = concept.FindLabel(preferredLabelRole),
                HypercubeDimensionNames = hypercubeDimensionNames
            };

            var outgoingArcs = node.GetOrderedOutgoingHierarchicalArcs<PresentationArc>();

            var attachedHypercube = GetAttachedHypercube(node);
            if (attachedHypercube != null)
            {
                if (outgoingArcs.Count != 1)
                {
                    throw new InvalidOperationException($"The hypercube {attachedHypercube.HypercubeItem.Name} has {outgoingArcs.Count} line items children, but exactly one is expected.");
                }

                var outgoingArc = outgoingArcs.Single();

                // We are replicating and adding the line items tree for each axis, including the respective dimension name to each concept member.
                foreach (var hypercubeAxis in attachedHypercube.Axes)
                {
                    var childHypercubeDimensionNames = new HashSet<XName>(hypercubeDimensionNames) {hypercubeAxis.Dimension.Name};
                    var child = TraversePresentationNetwork(outgoingArc.ToLocator, outgoingArc.PreferredLabelRole, childHypercubeDimensionNames);
                    child.HypercubeAxis = hypercubeAxis;
                    member.AddChild(child);
                }
            }
            else
            {
                foreach (var outgoingArc in outgoingArcs)
                {
                    var child = TraversePresentationNetwork(outgoingArc.ToLocator, outgoingArc.PreferredLabelRole, hypercubeDimensionNames);
                    member.AddChild(child);
                }
            }

            return member;
        }

        private Axis BuildHypercubeAxis(LocatorNode dimensionNode)
        {
            var rootNodes = dimensionNode.FromArcs.OfType<DefinitionArc>()
                .Where(a => a.Role == ArcRoles.DimensionToDomain)
                .Select(a => a.ToLocator)
                .ToList();

            // Just like for presentation networks, the IFRS taxonomy only has single roots for hypercube dimensions.
            if (rootNodes.Count > 1)
            {
                throw new InvalidOperationException($"The hypercube dimension {dimensionNode.Item.Name} has multiple root nodes.");
            }

            var dimension = new Dimension(dimensionNode.Item.Name);
            var rootMembers = rootNodes.Select(n => TraverseHypercubeDimension(n, dimension)).ToList();
            return new Axis(dimension, dimension.Name, rootMembers);
        }

        private ExplicitMember TraverseHypercubeDimension(LocatorNode node, Dimension dimension)
        {
            var memberItem = node.Item;
            var aspect = new ExplicitMemberAspect(dimension, memberItem.Name);
            var member = new ExplicitMember(aspect)
            {
                Item = memberItem,
                Label = memberItem.FindLabel(LabelRoles.Standard)
            };

            var outgoingArcs = node.GetOrderedOutgoingHierarchicalArcs<DefinitionArc>();

            foreach (var outgoingArc in outgoingArcs)
            {
                var child = TraverseHypercubeDimension(outgoingArc.ToLocator, dimension);
                member.AddChild(child);
            }

            return member;
        }
    }
}