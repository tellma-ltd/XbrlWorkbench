using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// Definition arcs define hypercube dimensions from XBRL Dimensions.
    /// Arc roles connect location nodes in the following way:
    /// (Concept item) - all - (hypercube item) - hypercube-dimension - (dimension item) - dimension-domain - (member item) - domain-member - (member item)
    /// </summary>
    /// <remarks>
    /// 1. The "all" arc role has two special attributes: closed and contextElement.
    /// IFRS taxonomies always set them to their recommended values closed="true" and contextElement="scenario",
    /// so we can safely ignore them.
    /// 2. There is also the "notAll" arc role for negative cubes which IFRS does not use.
    /// </remarks>
    public class DefinitionArc : HierarchicalArc
    {
        public DefinitionArc(XElement xArc) : base(xArc)
        {
        }

        public DefinitionArc(LocatorNode from, LocatorNode to, double order) : base(ArcRoles.DomainToMember, from, to, order)
        {
        }

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(ArcXNames.Definition);
            Fill(xLocatorNode);
            return xLocatorNode;
        }

    }
}