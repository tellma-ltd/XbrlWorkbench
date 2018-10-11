using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// A label arc connects a location node with a label node.
    /// </summary>
    public class LabelArc : Arc
    {
        public LabelArc(XElement xArc) : base(xArc)
        {
        }

        public LabelArc(LocatorNode from, LabelNode to) : base(ArcRoles.ConceptToLabel, from, to)
        {
            
        }

        public LocatorNode FromLocator => From as LocatorNode;

        public LabelNode ToLabelNode => To as LabelNode;

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(ArcXNames.Label);
            Fill(xLocatorNode);
            return xLocatorNode;
        }

    }

}