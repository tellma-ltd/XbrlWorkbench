using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// A reference arc connects a location node with a reference node.
    /// </summary>
    public class ReferenceArc : Arc
    {
        public ReferenceArc(XElement xArc) : base(xArc)
        {
        }

        public LocatorNode FromLocator => From as LocatorNode;

        public ReferenceNode ToReference => To as ReferenceNode;

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(ArcXNames.Reference);
            Fill(xLocatorNode);
            return xLocatorNode;
        }

    }

}