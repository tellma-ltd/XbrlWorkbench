using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// A calculation arc connects a parent location node with a child location node.
    /// </summary>
    public class CalculationArc : InterConceptArc
    {
        /// <summary>
        /// In practice either +1.0 or -1.0.
        /// </summary>
        public double Weight { get; }

        public CalculationArc(XElement xArc) : base(xArc)
        {
            Weight = double.Parse(xArc.Attribute(ArcXNames.Weight).Value);
        }

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(ArcXNames.Calculation,
                new XAttribute(ArcXNames.Weight, Weight)
            );

            Fill(xLocatorNode);
            return xLocatorNode;
        }

    }
}