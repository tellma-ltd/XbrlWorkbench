using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public abstract class Arc
    {
        public string FromLabel { get; set; }
        public string ToLabel { get; set; }


        public Node From { get; set; }
        public Node To { get; set; }

        public Uri Role { get; set; }

        public Link Link { get; set; }

        protected Arc(XElement xArc)
        {
            Role = new Uri(xArc.Attribute(Namespaces.XLink + "arcrole").Value);

            FromLabel = xArc.Attribute(Namespaces.XLink + "from").Value;
            ToLabel = xArc.Attribute(Namespaces.XLink + "to").Value;
        }

        protected Arc(Uri role, Node from, Node to)
        {
            Role = role;

            FromLabel = from.Label;
            ToLabel = to.Label;
        }

        protected virtual void Fill(XElement xNode)
        {
            xNode.Add(new XAttribute(XLinkXNames.From, FromLabel));
            xNode.Add(new XAttribute(XLinkXNames.To, ToLabel));
            xNode.Add(new XAttribute(XLinkXNames.Type, XLinkTypes.Arc));
            xNode.Add(new XAttribute(XLinkXNames.Role, Role));
        }

        public abstract XElement ToXml();

    }
}