using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class Schema : TaxonomyFile
    {
        public IList<RoleType> RoleTypes { get; }

        public IList<Item> Items { get; }

        public XAttribute TargetNamespaceDeclaration { get; }


        public Schema(Uri fileName, XAttribute targetNamespaceDeclaration) : base(fileName)
        {
            TargetNamespaceDeclaration = targetNamespaceDeclaration;
            RoleTypes = new List<RoleType>();
            Items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            item.Schema = this;
            Items.Add(item);
        }

        public void AddRoleType(RoleType roleType)
        {
            RoleTypes.Add(roleType);
            roleType.Schema = this;
        }

        public override XDocument ToXml()
        {
            var xDocument = new XDocument();
            var xSchema = new XElement(SchemaXNames.Schema);
            xDocument.Add(xSchema);

            AddTargetNamespace(xSchema);

            xSchema.Add(new XAttribute(XNamespace.Xmlns + "link", Namespaces.Link));
            xSchema.Add(new XAttribute(XNamespace.Xmlns + "xlink", Namespaces.XLink));
            xSchema.Add(new XAttribute(XNamespace.Xmlns + "xsd", Namespaces.Xsd));
            xSchema.Add(new XAttribute(XNamespace.Xmlns + "xbrli", Namespaces.Xbrli));

            var xAppInfo = CreateAppInfo();
            xSchema.Add(new XElement(SchemaXNames.Annotation, xAppInfo));

            xSchema.Add(CreateImports());
            xSchema.Add(Items.Select(i => i.ToXml()));

            return xDocument;
        }

        private XElement CreateAppInfo()
        {
            var xAppInfo = new XElement(SchemaXNames.AppInfo);
            xAppInfo.Add(RoleTypes.Select(rt => rt.ToXml()));
            xAppInfo.Add(Dependencies.OfType<Linkbase>().Select(lb => lb.CreateLinkbaseRef()));
            return xAppInfo;
        }

        private void AddTargetNamespace(XElement xSchema)
        {
            xSchema.Add(new XAttribute(SchemaXNames.TargetNamespace, TargetNamespaceDeclaration.Value));
            xSchema.Add(TargetNamespaceDeclaration);
        }

        public XElement CreateImport()
        {
            return new XElement(ImportXNames.Import,
                new XAttribute(ImportXNames.Namespace, TargetNamespaceDeclaration.Value),
                new XAttribute(ImportXNames.SchemaLocation, FileName));
        }

        public Uri CreateReference(string id)
        {
            var taxonomyEntryPoint = Taxonomy.EntryPointUri;
            if (taxonomyEntryPoint.IsAbsoluteUri)
            {
                var schemaReference = new Uri(taxonomyEntryPoint, FileName);
                return new Uri($"{schemaReference.AbsoluteUri}#{id}");
            }

            return new Uri($"{FileName}#{id}", UriKind.Relative);
        }

    }
}