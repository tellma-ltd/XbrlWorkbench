using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies;

namespace Banan.Tools.Xbrl.Instances
{
    public class Instance
    {
        public Instance()
        {
            Dts = new DiscoverableTaxonomySet();

            Entities = new List<Entity>();
            Periods = new List<Period>();
            Units = new List<Unit>();
            Facts = new List<Fact>();
        }

        /// <summary>
        /// The DTS is the set of all taxonomies that are linked to an instance.
        /// </summary>
        public DiscoverableTaxonomySet Dts { get; }

        public IList<Entity> Entities { get; }

        public IList<Period> Periods { get; }

        public IList<Unit> Units { get; }

        public IList<Fact> Facts { get; }

        public XDocument ToXml()
        {
            var xbrlElement = new XElement(Namespaces.Xbrli + "xbrl",
                new XAttribute(XNamespace.Xmlns + "xbrli", Namespaces.Xbrli),
                new XAttribute(XNamespace.Xmlns + "ix", Namespaces.Ix),
                new XAttribute(XNamespace.Xmlns + "link", Namespaces.Link),
                new XAttribute(XNamespace.Xmlns + "xlink", Namespaces.XLink),
                new XAttribute(XNamespace.Xmlns + "ISO4217", Namespaces.Iso4217)
            );

            Dts.AddItemNamespaceDeclarations(xbrlElement);

            xbrlElement.Add(GetSchemaReferenceElements());
            xbrlElement.Add(GetContextElements(xbrlElement));
            xbrlElement.Add(GetUnitElements());
            xbrlElement.Add(GetFactElements(xbrlElement));

            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xbrlElement);
            return document;
        }

        public XElement GetIxbrlHeaderElement(XElement namespaceDeclarationsElement)
        {
            var referencesElement = new XElement(Namespaces.Ix + "references");
            referencesElement.Add(GetSchemaReferenceElements());

            var resourcesElement = new XElement(Namespaces.Ix + "resources");
            resourcesElement.Add(GetContextElements(namespaceDeclarationsElement));
            resourcesElement.Add(GetUnitElements());

            var headerElement = new XElement(Namespaces.Ix + "header",
                referencesElement,
                resourcesElement);
            return headerElement;
        }

        private IEnumerable<XElement> GetSchemaReferenceElements()
        {
            foreach (var taxonomy in Dts.Taxonomies)
            {
                 yield return new XElement(Namespaces.Link + "schemaRef",
                    new XAttribute(Namespaces.XLink + "type", "simple"),
                    new XAttribute(Namespaces.XLink + "href", taxonomy.EntryPointUri));
            }
        }

        private IEnumerable<XElement> GetContextElements(XElement namespaceDeclarationsElement)
        {
            var contextIdGroups = Facts.GroupBy(f => f.GetContextId(namespaceDeclarationsElement)).ToList();
            var contexts = contextIdGroups.Select(g => g.First().Context);
            return contexts.Select(c => c.ToXml(namespaceDeclarationsElement));
        }

        private IEnumerable<XElement> GetUnitElements()
        {
            var units = Facts.Select(f => f.Unit).Distinct();
            return units.Select(c => c.ToXml());
        }

        private IEnumerable<XElement> GetFactElements(XElement namespaceDeclarationsElement)
        {
            return Facts.Select(c => c.ToXml(namespaceDeclarationsElement));
        }

        public void Save(IInstanceWriter writer)
        {
            writer.Write(this);
        }
    }
}