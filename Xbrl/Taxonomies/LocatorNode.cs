using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class LocatorNode : Node
    {
        public Uri Reference { get; }

        public Item Item { get; set; }

        public LocatorNode(XElement xNode, DiscoverableTaxonomySet dts) : base(xNode)
        {
            Reference =  new Uri(xNode.Attribute(XLinkXNames.Href).Value, UriKind.RelativeOrAbsolute);

            var idPart = Reference.OriginalString.Substring(Reference.OriginalString.IndexOf("#") + 1);
            var item = dts.FindItem(idPart);
            if (item == null)
            {
                throw new ArgumentException($"The item {idPart} was not found in the DTS.");
            }
            Connect(item);
        }

        public LocatorNode(string label, Item item) : base(label)
        {
            Reference = item.GetReference();
            Connect(item);
        }

        public IList<TArc> GetOrderedOutgoingHierarchicalArcs<TArc>() where TArc: HierarchicalArc
        {
            // Drop back to item to include arcs from different links with the same role. This happens with extension taxonomies.
            var fromArcs = Item.Nodes.SelectMany(n => n.FromArcs)
                .OfType<TArc>()
                .Where(a => a.Link.Role == Link.Role);

            return fromArcs
                .Where(a => a.ToLocator.Item.SubstitutionGroup == SubstitutionGroups.Item) // This check is needed because the presentation network also includes the hypercube dimensions. How odd!
                .OrderBy(a => a.Order)
                .ToList();
        }

        private void Connect(Item item)
        {
            Item = item;
            item.Nodes.Add(this);
        }

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(NodeXNames.Locator,
                new XAttribute(XLinkXNames.Type, XLinkTypes.Locator),
                new XAttribute(XLinkXNames.Href, Reference)
                );

            Fill(xLocatorNode);
            return xLocatorNode;
        }
    }
}