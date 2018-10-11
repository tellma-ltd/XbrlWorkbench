using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class LabelNode : Node
    {
        public LabelNode(XElement xNode) : base(xNode)
        {
            Role = new Uri(xNode.Attribute(XLinkXNames.Role).Value);

            if (Role == LabelRoles.Documentation)
            {
                
            }


            Language = xNode.Attribute(XmlXNames.Lang).Value;
            Text = xNode.Value;
        }

        public LabelNode(string label, Uri role, string text, string language) : base(label)
        {
            Role = role;
            Text = text;
            Language = language;
        }

        /// <summary>
        /// One of the many roles defined in the LabelRoles list.
        /// </summary>
        public Uri Role { get; }

        public string Text { get; }

        public string Language { get; set; }

        public override XElement ToXml()
        {
            var xLocatorNode = new XElement(NodeXNames.Label,
                new XAttribute(XLinkXNames.Type, XLinkTypes.Resource),
                new XAttribute(XLinkXNames.Role, Role),
                new XAttribute(XmlXNames.Lang, Language),
                Text
            );

            Fill(xLocatorNode);
            return xLocatorNode;
        }

    }
}