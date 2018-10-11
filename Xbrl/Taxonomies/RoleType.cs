using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    /// Defined in the xs:appInfo element.
    /// Provides additional information about a link role.
    /// </summary>
    public class RoleType
    {
        public Uri Role { get; }

        public string Id { get; set; }

        /// <summary>
        /// This is a human-friendly title for the network.
        /// </summary>
        public string Definition { get; }

        /// <summary>
        /// The values are linkbase types from the LinkbaseXNames list.
        /// </summary>
        public IList<string> UsedOn { get; set; }

        public Schema Schema { get; set; }

        public RoleType(XElement xRoleType)
        {
            Id = xRoleType.Attribute(RoleTypeXNames.Id).Value;
            Role = new Uri(xRoleType.Attribute(RoleTypeXNames.Role).Value);
            Definition = xRoleType.Element(RoleTypeXNames.Definition).Value;

            UsedOn = xRoleType.Elements(RoleTypeXNames.UsedOn).Select(e => e.Value).ToList();
        }

        public XElement ToXml()
        {
            var xRoleType = new XElement(RoleTypeXNames.RoleType,
                new XAttribute(RoleTypeXNames.Id, Id),
                new XAttribute(RoleTypeXNames.Role, Role),
                new XElement(RoleTypeXNames.Definition, Definition)
                );

            xRoleType.Add(UsedOn.Select(uo => new XElement(RoleTypeXNames.UsedOn, uo)));

            return xRoleType;
        }

        public Uri GetReference()
        {
            return Schema.CreateReference(Id);
        }

    }
}