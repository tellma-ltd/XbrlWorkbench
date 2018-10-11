using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class Linkbase : TaxonomyFile
    {
        public IList<Link> Links { get; }

        public Linkbase(Uri fileName) : base(fileName)
        {
            Links = new List<Link>();
        }

        public void AddLink(Link link, DiscoverableTaxonomySet dts)
        {
            link.RoleType = dts.FindRoleType(link.Role);
            Links.Add(link);
        }

        public override XDocument ToXml()
        {
            var xDocument = new XDocument();
            var xLinkbase = new XElement(LinkbaseXNames.Linkbase);
            xDocument.Add(xLinkbase);

            xLinkbase.Add(new XAttribute(XNamespace.Xmlns + "link", Namespaces.Link));
            xLinkbase.Add(new XAttribute(XNamespace.Xmlns + "xlink", Namespaces.XLink));
            xLinkbase.Add(new XAttribute(XNamespace.Xmlns + "xsi", Namespaces.Xsi));
            xLinkbase.Add(new XAttribute(LinkbaseXNames.SchemaLocation, LinkbaseXNames.SchemaLocationValue));

            AddLinkReferences(xLinkbase);

            var xLinks = Links.Select(l => l.ToXml());
            xLinkbase.Add(xLinks);

            return xDocument;
        }

        private void AddLinkReferences(XElement xLinkbase)
        {
            foreach (var link in Links.Where(l => l.RoleType != null))
            {
                var roleType = link.RoleType;
                var xLinkRef = new XElement(RoleRefXNames.RoleRef,
                    new XAttribute(RoleRefXNames.Role, roleType.Role),
                    new XAttribute(XLinkXNames.Href, roleType.GetReference()),
                    new XAttribute(XLinkXNames.Type, XLinkTypes.Simple));

                xLinkbase.Add(xLinkRef);
            }
        }

        public XElement CreateLinkbaseRef()
        {
            var xRef = new XElement(LinkbaseRefXNames.LinkbaseRef,
                new XAttribute(XLinkXNames.ArcRole, LinkbaseRefXNames.ArcRoleValue),
                new XAttribute(XLinkXNames.Type, XLinkTypes.Simple),
                new XAttribute(XLinkXNames.Href, FileName));

            var groupedByRefRole = Links.GroupBy(l => l.ReferenceRole).ToList();
            if (groupedByRefRole.Count==1)
            {
                xRef.Add(new XAttribute(XLinkXNames.Role, groupedByRefRole.Single().Key));
            }

            return xRef;
        }

    }
}