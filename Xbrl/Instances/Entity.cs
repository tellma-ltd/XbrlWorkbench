using System;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances
{
    public class Entity
    {
        /// <summary>
        /// The Id only serves to generate unique context ids. It is not part of the XBRL specification.
        /// </summary>
        /// <example>
        /// toys for Toys'R'Us
        /// </example>
        public string Id { get; set; }

        /// <summary>
        /// The natural name of the entity. This is not part of the XBRL specification
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A token that is a valid identifier within the namespace referenced by the IdentifierScheme.
        /// </summary>
        /// <example>
        /// 0001005414 for Toys'R'Us
        /// </example>
        public string Identifier { get; set; }

        /// <summary>
        /// The scheme used to identify business entities.
        /// See https://stackoverflow.com/questions/26650649/entity-identifier-scheme-url-in-xbrl
        /// </summary>
        /// <example>
        /// http://www.sec.gov/CIK
        /// </example>
        public Uri IdentifierScheme { get; set; }


        public XElement ToXml()
        {
            var xIdentifier = new XElement(Namespaces.Xbrli + "identifier",
                new XAttribute("scheme", IdentifierScheme), Identifier);
            return new XElement(Namespaces.Xbrli + "entity", xIdentifier);
        }
    }
}