using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// A presentation arc connects a parent location node with a child location node.
    /// </summary>
    public class PresentationArc : HierarchicalArc
    {
        /// <summary>
        /// The preferred label role for the ToLocation.
        /// </summary>
        public Uri PreferredLabelRole { get; }

        public PresentationArc(XElement xArc) : base(xArc)
        {
            var preferredLabelRoleValue = xArc.Attribute(ArcXNames.PreferredLabelRole)?.Value;
            PreferredLabelRole = preferredLabelRoleValue == null ? LabelRoles.Standard : new Uri(preferredLabelRoleValue);
        }

        public PresentationArc(LocatorNode from, LocatorNode to, double order, Uri preferredLabelRole) : base(ArcRoles.ParentToChild, from, to, order)
        {
            PreferredLabelRole = preferredLabelRole;
        }

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(ArcXNames.Definition,
                new XAttribute(ArcXNames.PreferredLabelRole, PreferredLabelRole));
            Fill(xLocatorNode);
            return xLocatorNode;
        }

    }
}