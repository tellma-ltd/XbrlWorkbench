using System.Xml.Linq;
using Banan.Tools.Xbrl.Instances.Export.Writers;

namespace Banan.Tools.Xbrl.Instances
{
    public class ExplicitMember
    {
        /// <summary>
        /// The dimension (aka axis in the IFRS taxonomy) name.
        /// </summary>
        /// <example>{http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}ClassesOfPropertyPlantAndEquipmentAxis</example>
        public XName DimensionName { get; }

        /// <summary>
        /// The member name.
        /// </summary>
        /// <example>{http://xbrl.ifrs.org/taxonomy/2017-03-09/ifrs-full}MotorVehiclesMember</example>
        public XName MemberName { get; }

        public ExplicitMember(XName dimensionName, XName memberName)
        {
            DimensionName = dimensionName;
            MemberName = memberName;
        }

        public XElement ToXml(XElement namespaceDeclarationsElement)
        {
            var xExplicitMember = new XElement(Namespaces.Xbrldi + "explicitMember");
            xExplicitMember.Value = MemberName.ToColonSeparated(namespaceDeclarationsElement);
            xExplicitMember.Add(new XAttribute("dimension", DimensionName.ToColonSeparated(namespaceDeclarationsElement)));
            return xExplicitMember;
        }
    }
}