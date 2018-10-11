using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    /// <summary>
    ///     (Extended) links are contained inside XBRL linkbases and come in several flavours like
    ///     presentation links, label links, calculation links and more.
    ///     XBRL links may contain locators, resources and arc
    ///     (in addition to documentations and titles, which are optional and don't hold any XBRL semantics)
    /// </summary>
    public abstract class Link
    {
        protected Link(XName elementName, Uri role)
        {
            Role = role;
            ElementName = elementName;

            NodesMap = new Dictionary<string, Node>();
            Arcs = new List<Arc>();
        }

        /// <summary>
        ///     For presentation links, the role is a network identifier, e.g.
        ///     http://xbrl.ifrs.org/role/ifrs/ias_1_2017-03-09_role-210000
        ///     For label links, the Role is the generic http://www.xbrl.org/2003/role/link.
        /// </summary>
        public Uri Role { get; }

        /// <summary>
        ///     Some links roles are described by an associated role type.
        /// </summary>
        public RoleType RoleType { get; set; }

        /// <summary>
        ///     To identify the type of link: calculationLink, labelLink or presentationLink.
        /// </summary>
        public XName ElementName { get; }

        public IDictionary<string, Node> NodesMap { get; }

        public IList<Arc> Arcs { get; }

        public abstract Uri ReferenceRole { get; }

        public void AddNode(Node node)
        {
            if (node.Link != null)
            {
                throw new ArgumentException("The node already belongs to a link.");
            }

            node.Link = this;

            if (NodesMap.ContainsKey(node.Label))
            {
                throw new ArgumentException($"The node label {node.Label} already exists in the link {Role}. This is allowed in the XBRL specification (chapter 3.5.3.7.3), but not used in the IFRS taxonomies and not supported in this XBRL implementation.");
            }

            NodesMap[node.Label] = node;
        }

        public void AddArc(Arc arc)
        {
            if (arc.Link != null)
            {
                throw new ArgumentException("The arc already belongs to a link.");
            }

            arc.Link = this;
            Arcs.Add(arc);

            Connect(arc);
        }

        private void Connect(Arc arc)
        {
            var from = NodesMap[arc.FromLabel];
            arc.From = from;
            from.FromArcs.Add(arc);

            var to = NodesMap[arc.ToLabel];
            arc.To = to;
            to.ToArcs.Add(arc);
        }

        public virtual XElement ToXml()
        {
            var xLink = new XElement(ElementName,
                new XAttribute(XLinkXNames.Type, XLinkTypes.Extended),
                new XAttribute(XLinkXNames.Role, Role)
                );

            var xNodes = NodesMap.Values.Select(n => n.ToXml());
            xLink.Add(xNodes);

            var xArcs = Arcs.Select(a => a.ToXml());
            xLink.Add(xArcs);

            return xLink;
        }
    }
}