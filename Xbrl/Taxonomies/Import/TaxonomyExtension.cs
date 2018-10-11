using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class TaxonomyExtension : ITaxonomySource
    {
        private readonly XAttribute _targetNamespaceDeclaration;

        public IList<ExtensionItem> ExtensionItems;

        public TaxonomyExtension(string targetNamespacePrefix, XNamespace targetNamespace) : this(new XAttribute(XNamespace.Xmlns + targetNamespacePrefix, targetNamespace))
        {

        }

        private TaxonomyExtension(XAttribute targetNamespaceDeclaration)
        {
            if (!targetNamespaceDeclaration.IsNamespaceDeclaration)
            {
                throw new ArgumentException("The provided item namespace declaration is not a namespace declaration.", nameof(targetNamespaceDeclaration));
            }

            _targetNamespaceDeclaration = targetNamespaceDeclaration;

            ExtensionItems = new List<ExtensionItem>();
        }

        #region ITaxonomySource

        public void Fill(Taxonomy taxonomy, DiscoverableTaxonomySet dts)
        {
            if (taxonomy.EntryPointUri.IsAbsoluteUri)
            {
                throw new ArgumentException("The entry point for an extension taxonomy must be relative.");
            }

            var schema = CreateSchema(taxonomy.EntryPointUri);
            taxonomy.SetEntryPoint(schema);

            var labelLinkbase = CreateLabelLinkbase(taxonomy, dts);
            schema.Add(labelLinkbase);

            var presentationLinkbase = CreatePresentationLinkbase(taxonomy, dts);
            schema.Add(presentationLinkbase);

            var definitionLinkbase = CreateDefinitionLinkbase(taxonomy, dts);
            schema.Add(definitionLinkbase);
        }

        #endregion

        private Schema CreateSchema(Uri entryPoint)
        {
            var schema = new Schema(entryPoint, _targetNamespaceDeclaration);
            foreach (var extensionItem in ExtensionItems)
            {
                var item = CreateItem(extensionItem);
                schema.AddItem(item);
            }

            return schema;
        }

        private Item CreateItem(ExtensionItem extensionItem)
        {
            var item = new Item
            {
                Id = CreateId(extensionItem),
                Name = (XNamespace) _targetNamespaceDeclaration.Value + extensionItem.Name,
                PeriodType = extensionItem.PeriodType,
                BalanceType = extensionItem.BalanceType,
                DataType = extensionItem.DataType,
                IsAbstract = extensionItem.IsAbstract,
                SubstitutionGroup = extensionItem.SubstitutionGroup
            };
            return item;
        }

        private string CreateId(ExtensionItem item)
        {
            return $"{_targetNamespaceDeclaration.Name.LocalName}_{item.Name}";
        }

        private Linkbase CreateLabelLinkbase(Taxonomy taxonomy, DiscoverableTaxonomySet dts)
        {
            var linkbase = new Linkbase(GetLinkbaseFileName(taxonomy, "lab"));

            var labelLink = new LabelLink(LinkbaseXNames.Label, LinkRoles.Generic);
            linkbase.AddLink(labelLink, dts);

            var locCount = 0;
            var labCount = 0;
            foreach (var extensionItem in ExtensionItems)
            {
                var locLabel = $"loc{locCount}";
                locCount += 1;
                var locNode = CreateLocatorNode(locLabel, extensionItem, dts);
                labelLink.AddNode(locNode);

                foreach (var extensionLabel in extensionItem.Labels)
                {
                    var labLabel = $"lab{labCount}";
                    labCount += 1;
                    var labNode = new LabelNode(labLabel, extensionLabel.Role, extensionLabel.Text, extensionLabel.Language);
                    labelLink.AddNode(labNode);

                    var arc = new LabelArc(locNode, labNode);
                    labelLink.AddArc(arc);
                }
            }

            return linkbase;
        }

        private Linkbase CreatePresentationLinkbase(Taxonomy taxonomy, DiscoverableTaxonomySet dts)
        {
            var conceptItems = ExtensionItems.OfType<ExtensionConcept>().ToList(); // Does that include abstracts?

            var linkbase = new Linkbase(GetLinkbaseFileName(taxonomy, "pre"));

            var groupedByNetworkRole = conceptItems.SelectMany(i => i.Locations.Select(l =>
                    new {l.NetworkRole, Location = l, Item = i}))
                .GroupBy(d => d.NetworkRole)
                .ToList();

            foreach (var groupByNetworkRole in groupedByNetworkRole)
            {
                var presentationLink = new PresentationLink(LinkbaseXNames.Presentation, groupByNetworkRole.Key);
                linkbase.AddLink(presentationLink, dts);

                var locCount = 0;
                foreach (var locationAndItem in groupByNetworkRole)
                {
                    var parentItem = dts.FindItem(locationAndItem.Location.ParentName);
                    if (parentItem == null)
                    {
                        throw new InvalidOperationException($"There is no concept {locationAndItem.Location.ParentName} in the DTS.");
                    }

                    var parentLocatorNodes = parentItem.Nodes
                        .Where(n => n.Link.Role == groupByNetworkRole.Key).ToList();
                    if (!parentLocatorNodes.Any())
                    {
                        throw new InvalidOperationException($"The concept {locationAndItem.Location.ParentName} is not in the presentation network {groupByNetworkRole.Key}.");
                    }

                    var siblingArcs = parentLocatorNodes.First().GetOrderedOutgoingHierarchicalArcs<PresentationArc>();

                    double newOrder;
                    try
                    {
                        newOrder = DetermineOrder(locationAndItem.Location.PrecedingSiblingName, siblingArcs);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException($"The sibling {locationAndItem.Location.PrecedingSiblingName} was not found as a child of {locationAndItem.Location.ParentName} in the presentation network {groupByNetworkRole.Key}.", ex);
                    }

                    var parentLocLabel = $"loc_{locCount}";
                    locCount += 1;
                    var parentNode = new LocatorNode(parentLocLabel, parentItem);
                    presentationLink.AddNode(parentNode);

                    var childLocLabel = $"loc_{locCount}";
                    locCount += 1;
                    var childNode = CreateLocatorNode(childLocLabel, locationAndItem.Item, dts);
                    presentationLink.AddNode(childNode);

                    var arc = new PresentationArc(parentNode, childNode, newOrder, locationAndItem.Location.PreferredLabelRole);
                    presentationLink.AddArc(arc);
                }
            }

            return linkbase;
        }

        private LocatorNode CreateLocatorNode(string label, ExtensionItem extensionItem, DiscoverableTaxonomySet dts)
        {
            var itemName = (XNamespace) _targetNamespaceDeclaration.Value + extensionItem.Name;
            var item = dts.FindItem(itemName);
            if (item == null)
            {
                throw new InvalidOperationException($"There is no item called {itemName} in the DTS. Perhaps you added taxonomies in the wrong order or forgot to add a base taxonomy.");
            }
            return new LocatorNode(label, item);
        }

        private Linkbase CreateDefinitionLinkbase(Taxonomy taxonomy, DiscoverableTaxonomySet dts)
        {
            var linkbase = new Linkbase(GetLinkbaseFileName(taxonomy, "def"));

            var memberItems = ExtensionItems.OfType<ExtensionMember>().ToList();

            var groupedByHypercube = memberItems.SelectMany(i => i.Locations.Select(l =>
                    new {l.HypercubeName, Location = l, Item = i}))
                .GroupBy(d => d.HypercubeName)
                .ToList();

            foreach (var groupByHypercube in groupedByHypercube)
            {
                var hypercubeNode = dts.GetAllLinks()
                    .Where(l => l.ElementName == LinkbaseXNames.Definition)
                    .SelectMany(l => l.NodesMap.Values)
                    .OfType<LocatorNode>()
                    .SingleOrDefault(ln => ln.Item.Name == groupByHypercube.Key);
                if (hypercubeNode == null)
                {
                    throw new InvalidOperationException($"Could not find a hypercube (aka table) {groupByHypercube.Key} in the DTS.");
                }

                var definitionLink = new DefinitionLink(LinkbaseXNames.Definition, hypercubeNode.Link.Role);
                linkbase.AddLink(definitionLink, dts);

                var locCount = 0;
                foreach (var locationAndItem in groupByHypercube)
                {
                    var dimensionLocationNode = hypercubeNode.FromArcs
                        .OfType<DefinitionArc>()
                        .Select(da => da.ToLocator)
                        .SingleOrDefault(ln => ln.Item.Name == locationAndItem.Location.DimensionName);

                    if (dimensionLocationNode == null)
                    {
                        throw new InvalidOperationException($"Could not find a dimension (aka axis) {locationAndItem.Location.DimensionName} in the hypercube (aka table) {groupByHypercube.Key}.");
                    }

                    var parentNodeInBaseTaxonomy = FindParentNode(dimensionLocationNode, locationAndItem.Location.ParentName);
                    if (parentNodeInBaseTaxonomy == null)
                    {
                        throw new InvalidOperationException($"There is no member {locationAndItem.Location.ParentName} in the dimension {groupByHypercube.Key}.");
                    }

                    var siblingArcs = parentNodeInBaseTaxonomy.GetOrderedOutgoingHierarchicalArcs<DefinitionArc>();

                    double newOrder;
                    try
                    {
                        newOrder = DetermineOrder(locationAndItem.Location.PrecedingSiblingName, siblingArcs);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException($"The sibling {locationAndItem.Location.PrecedingSiblingName} was not found as a child of {locationAndItem.Location.ParentName} in the dimension {groupByHypercube.Key}.", ex);
                    }

                    var parentLocLabel = $"loc_{locCount}";
                    locCount += 1;
                    var parentNode = new LocatorNode(parentLocLabel, parentNodeInBaseTaxonomy.Item);
                    definitionLink.AddNode(parentNode);

                    var locLabel = $"loc_{locCount}";
                    locCount += 1;
                    var locNode = CreateLocatorNode(locLabel, locationAndItem.Item, dts);
                    definitionLink.AddNode(locNode);

                    var arc = new DefinitionArc(parentNode, locNode, newOrder);
                    definitionLink.AddArc(arc);
                }
            }

            return linkbase;
        }

        private LocatorNode FindParentNode(LocatorNode locatorNode, XName parentName)
        {
            if (locatorNode.Item.Name == parentName)
            {
                return locatorNode;
            }

            var outgoingArcs = locatorNode.GetOrderedOutgoingHierarchicalArcs<DefinitionArc>();
            foreach (var outgoingArc in outgoingArcs)
            {
                var childResult = FindParentNode(outgoingArc.ToLocator, parentName);
                if (childResult != null)
                {
                    return childResult;
                }
            }

            return null;
        }

        private double DetermineOrder<TArc>(XName precedingSiblingName, IList<TArc> siblingArcs) where TArc : HierarchicalArc
        {
            if (precedingSiblingName == null)
            {
                return siblingArcs.Any() ? siblingArcs.First().Order - 1 : 10.0;
            }

            var arcsFromPrecedingSibling = siblingArcs
                .SkipWhile(a => a.ToLocator.Item.Name != precedingSiblingName)
                .ToList();
            if (!arcsFromPrecedingSibling.Any())
            {
                throw new InvalidOperationException();
            }

            return arcsFromPrecedingSibling.Count == 1
                ? arcsFromPrecedingSibling.First().Order + 1
                : (arcsFromPrecedingSibling[1].Order + arcsFromPrecedingSibling[0].Order) / 2;
        }

        private Uri GetLinkbaseFileName(Taxonomy taxonomy, string suffix)
        {
            return new Uri($"{Path.GetFileNameWithoutExtension(taxonomy.EntryPointUri.OriginalString)}_{suffix}.xml", UriKind.Relative);
        }
    }
}
