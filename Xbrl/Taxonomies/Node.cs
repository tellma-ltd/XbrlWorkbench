using System.Collections.Generic;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public abstract class Node 
    {
        public Link Link { get; set; }

        /// <summary>
        /// Arcs link to nodes via the label.
        /// </summary>
        /// <remarks>
        /// Don't confuse with XBRL labels.
        /// </remarks>
        public string Label { get; }

        /// <summary>
        /// The list of arcs that point to this node via the From attribute.
        /// Linking back to arcs enables local operations, namely finding a concept's label
        /// and building up presentation and dimension trees.
        /// </summary>
        public IList<Arc> FromArcs { get; }

        /// <summary>
        /// The list of arcs that point to this node via the To attribute.
        /// </summary>
        public IList<Arc> ToArcs { get; }

        /// <summary>
        /// The Id stems from the underlying XLink resource and is optional. When set, they can assist in Arc prohibition.
        /// See 3.5.3.8.4 in the XBRL specifications.
        /// </summary>
        /// <remarks>
        /// The IFRS taxonomy sets it for labels but not for references.
        /// </remarks>
        public string Id { get; set; }


        private Node()
        {
            FromArcs = new List<Arc>();
            ToArcs = new List<Arc>();
        }


        protected Node(string label):this()
        {
            Label = label;
        }


        protected Node(XElement xNode) : this()
        {
            Label = xNode.Attribute(XLinkXNames.Label).Value;
            Id = xNode.Attribute("id")?.Value;
        }

        protected void Fill(XElement xNode)
        {
            xNode.Add(new XAttribute(XLinkXNames.Label, Label));
            if (Id != null)
            {
                xNode.Add(new XAttribute("id", Id));
            }
        }

        public abstract XElement ToXml();
    }
}