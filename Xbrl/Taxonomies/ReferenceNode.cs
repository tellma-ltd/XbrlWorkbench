using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class ReferenceNode : Node
    {
        public ReferenceNode(XElement xNode) : base(xNode)
        {
            Role = new Uri(xNode.Attribute(XLinkXNames.Role).Value);

            Name = xNode.Element(ReferencePartXNames.Name).Value;
            Number = int.Parse(xNode.Element(ReferencePartXNames.Number).Value);
            IssueDate = DateTime.ParseExact(xNode.Element(ReferencePartXNames.IssueDate).Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Paragraph = xNode.Element(ReferencePartXNames.Paragraph)?.Value;
            Subparagraph = xNode.Element(ReferencePartXNames.Subparagraph)?.Value;
            Section = xNode.Element(ReferencePartXNames.Section)?.Value;
            Subsection = xNode.Element(ReferencePartXNames.Subsection)?.Value;
            Clause = xNode.Element(ReferencePartXNames.Clause)?.Value;
            Uri = new Uri(xNode.Element(ReferencePartXNames.Uri).Value);
            UriDate = DateTime.ParseExact(xNode.Element(ReferencePartXNames.UriDate).Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Note = xNode.Element(ReferencePartXNames.Note)?.Value;
        }

        /// <summary>
        /// As listed in ReferenceRoles.
        /// </summary>
        public Uri Role { get; set; }

        #region Parts

        public string Name { get; set; }

        public int Number { get; set; }

        public DateTime? IssueDate { get; set; }

        /// <summary>
        ///     Can be of the form "6.6.4".
        /// </summary>
        public string Paragraph { get; set; }

        public string Subparagraph { get; set; }

        public string Section { get; set; }

        public string Subsection { get; set; }

        public string Clause { get; set; }

        public Uri Uri { get; set; }

        public DateTime? UriDate { get; set; }

        public string Note { get; set; }

        #endregion

        public override XElement ToXml()
        {
            var xReferenceNode = new XElement(NodeXNames.Reference,
                new XAttribute(XLinkXNames.Type, XLinkTypes.Resource),
                new XAttribute(XLinkXNames.Role, Role)
            );

            var partAttributes = new []
            {
                ToPartAttribute(Name, ReferencePartXNames.Name),
                ToPartAttribute(Number, ReferencePartXNames.Number),
                ToPartAttribute(IssueDate, ReferencePartXNames.IssueDate),
                ToPartAttribute(Paragraph, ReferencePartXNames.Paragraph),
                ToPartAttribute(Subparagraph, ReferencePartXNames.Subparagraph),
                ToPartAttribute(Section, ReferencePartXNames.Section),
                ToPartAttribute(Subsection, ReferencePartXNames.Subsection),
                ToPartAttribute(Clause, ReferencePartXNames.Clause),
                ToPartAttribute(Uri, ReferencePartXNames.Uri),
                ToPartAttribute(UriDate, ReferencePartXNames.UriDate),
                ToPartAttribute(Note, ReferencePartXNames.Note),
            };

            foreach (var partAttribute in partAttributes.Where(a => a != null))
            {
                xReferenceNode.Add(partAttribute);
            }

            Fill(xReferenceNode);
            return xReferenceNode;
        }

        private XAttribute ToPartAttribute(string value, XName name)
        {
            return value == null ? null : new XAttribute(name, value);
        }

        private XAttribute ToPartAttribute(int value, XName name)
        {
            return new XAttribute(name, value);
        }

        private XAttribute ToPartAttribute(Uri value, XName name)
        {
            return value == null ? null : new XAttribute(name, value);
        }

        private XAttribute ToPartAttribute(DateTime? nullableDateTime, XName name)
        {
            return nullableDateTime == null ? null : new XAttribute(name, nullableDateTime.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Name} {Number}");

            if (!string.IsNullOrEmpty(Section))
            {
                sb.Append($" section {Section}");
            }
            if (!string.IsNullOrEmpty(Subsection))
            {
                sb.Append($".{Subsection}");
            }

            if (!string.IsNullOrEmpty(Paragraph))
            {
                sb.Append($" paragraph {Paragraph}");
            }
            if (!string.IsNullOrEmpty(Subparagraph))
            {
                sb.Append($".{Subparagraph}");
            }

            if (!string.IsNullOrEmpty(Clause))
            {
                sb.Append($" clause {Clause}");
            }

            return sb.ToString();
        }
    }
}