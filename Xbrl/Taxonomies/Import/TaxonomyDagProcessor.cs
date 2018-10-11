using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    internal class TaxonomyDagProcessor
    {
        private readonly DiscoverableTaxonomySet _dts;
        private readonly IDictionary<Uri, TaxonomyFile> _processedFiles;
        private readonly IFileReader _fileReader;

        public TaxonomyDagProcessor(DiscoverableTaxonomySet dts, IFileReader fileReader)
        {
            _dts = dts;
            _fileReader = fileReader;
            _processedFiles = new Dictionary<Uri, TaxonomyFile>();
        }

        public TaxonomyFile ParseDocument(XDocument xDocument, Uri fileName)
        {
            var xRoot = xDocument.Root;
            if (xRoot==null)
            {
                return null;
            }

            if (xRoot.Name == SchemaXNames.Schema)
            {
                return ParseSchema(xRoot, fileName);
            }

            if (xRoot.Name == LinkbaseXNames.Linkbase)
            {
                return ParseLinkbase(xRoot, fileName);
            }

            return null;
        }

        private Linkbase ParseLinkbase(XElement xLinkbase, Uri fileName)
        {
            var linkbase = new Linkbase(fileName);

            var xLinks = xLinkbase.Elements()
                .Where(e => e.Attribute(XLinkXNames.Type)?.Value == XLinkTypes.Extended).ToList();

            foreach (var xLink in xLinks)
            {
                var link = CreateLink(xLink);

                if (link != null)
                {
                    linkbase.AddLink(link, _dts);

                    ParseNodes(xLink, link);
                    ParseArcs(xLink, link);
                }
            }

            return linkbase;
        }

        private Link CreateLink(XElement xLink)
        {
            var role = new Uri(xLink.Attribute(XLinkXNames.Role).Value);

            if (xLink.Name == LinkbaseXNames.Presentation)
            {
                return new PresentationLink(xLink.Name, role);
            }
            if (xLink.Name == LinkbaseXNames.Label)
            {
                return new LabelLink(xLink.Name, role);
            }
            if (xLink.Name == LinkbaseXNames.Definition)
            {
                return new DefinitionLink(xLink.Name, role);
            }
            if (xLink.Name == LinkbaseXNames.Calculation)
            {
                return new CalculationLink(xLink.Name, role);
            }
            if (xLink.Name == LinkbaseXNames.Reference)
            {
                return new ReferenceLink(xLink.Name, role);
            }

            // Unsupported link type
            return null;
        }

        private Schema ParseSchema(XElement xSchema, Uri fileName)
        {
            var targetNamespaceDeclaration = GetTargetNamespaceDeclaration(xSchema, fileName);
            var schema = new Schema(fileName, targetNamespaceDeclaration);

            var xElements = xSchema
                .Elements(ElementXNames.Element)
                .ToList();

            foreach (var xElement in xElements)
            {
                var item = new Item(xElement, targetNamespaceDeclaration.Value);
                schema.AddItem(item);
            }

            var roleTypeElements = xSchema
                .Descendants()
                .Where(e => e.Name == RoleTypeXNames.RoleType);

            foreach (var roleTypeElement in roleTypeElements)
            {
                var roleType = new RoleType(roleTypeElement);
                schema.AddRoleType(roleType);
            }

            return schema;
        }

        private XAttribute GetTargetNamespaceDeclaration(XElement xSchema, Uri fileName)
        {
            var targetNamespaceAttribute = xSchema.Attribute(SchemaXNames.TargetNamespace);
            if (targetNamespaceAttribute == null)
            {
                throw new InvalidOperationException($"There is no targetNamespace attribute in the {fileName} schema.");
            }

            var namespaceDeclaration = xSchema.Attributes().SingleOrDefault(a => a.IsNamespaceDeclaration && a.Value == targetNamespaceAttribute.Value);
            if (namespaceDeclaration == null)
            {
                throw new InvalidOperationException($"There is no namespace declaration for the targetNamespace {targetNamespaceAttribute.Value} in the {fileName} schema.");
            }

            return namespaceDeclaration;
        }

        private void ParseArcs(XElement xLink, Link link)
        {
            var xArcs = xLink.Descendants().Where(d => d.Attribute(XLinkXNames.ArcRole) != null).ToList();
            foreach (var xArc in xArcs)
            {
                var arc = CreateArc(xArc);
                if (arc != null)
                {
                    link.AddArc(arc);
                }
            }
        }

        private Arc CreateArc(XElement xArc)
        {
            if (xArc.Name == ArcXNames.Label)
            {
                return new LabelArc(xArc);
            }

            if (xArc.Name == ArcXNames.Calculation)
            {
                return new CalculationArc(xArc);
            }

            if (xArc.Name == ArcXNames.Presentation)
            {
                return new PresentationArc(xArc);
            }

            if (xArc.Name == ArcXNames.Reference)
            {
                return new ReferenceArc(xArc);
            }

            if (xArc.Name == ArcXNames.Definition)
            {
                return new DefinitionArc(xArc);
            }

            // Unsupported arc type
            return null;
        }

        private void ParseNodes(XElement linkElement, Link link)
        {
            foreach (var xNode in linkElement.Elements())
            {
                var node = CreateNode(xNode);
                if (node != null)
                {
                    link.AddNode(node);
                }
            }
        }

        private Node CreateNode(XElement xNode)
        {
            if (xNode.Name == NodeXNames.Locator)
            {
                return new LocatorNode(xNode, _dts);
            }

            if (xNode.Name == NodeXNames.Label)
            {
                return new LabelNode(xNode);
            }

            if (xNode.Name == NodeXNames.Reference)
            {
                return new ReferenceNode(xNode);
            }

            // Not a node or unsupported node.
            return null;
        }

        public void Process(Uri fileName, ITaxonomyFileHolder taxonomyFileHolder)
        {
            if (_processedFiles.ContainsKey(fileName))
            {
                taxonomyFileHolder.Add(_processedFiles[fileName]);
            }

            var xDocument = _fileReader.Read(fileName.OriginalString);

            if (xDocument == null)
            {
                throw new InvalidOperationException($"Could not find the file {fileName.OriginalString}.");
            }

            var taxonomyFile = ParseDocument(xDocument, fileName);
            _processedFiles[fileName] = taxonomyFile;

            if (taxonomyFile != null)
            {
                // Order is important: the taxonomy file must be added to the DTS before
                // processing dependencies.
                taxonomyFileHolder.Add(taxonomyFile);

                AddAbsoluteImports(taxonomyFile, xDocument);
                AddRelativeImports(taxonomyFile, xDocument, fileName);
            }
        }

        private void AddRelativeImports(TaxonomyFile taxonomyFile, XDocument xDocument, Uri fileName)
        {
            var relativeFileNames = ExtractRelativeFilenames(xDocument, fileName);
            foreach (var relativeFileName in relativeFileNames)
            {
                Process(relativeFileName, taxonomyFile);
            }
        }

        private void AddAbsoluteImports(TaxonomyFile taxonomyFile, XDocument xDocument)
        {
            var xAbsoluteImports = xDocument.Root
                .Elements(ImportXNames.Import)
                .Where(i => i.Attribute(ImportXNames.SchemaLocation).Value.StartsWith("http:"));

            foreach (var xAbsoluteImport in xAbsoluteImports)
            {
                var namespaceValue = xAbsoluteImport.Attribute(ImportXNames.Namespace).Value;
                var schemaLocationValue = xAbsoluteImport.Attribute(ImportXNames.SchemaLocation).Value;
                var targetNamespaceDeclaration = new XAttribute(XNamespace.Xmlns + "ignore", namespaceValue);
                var schema = new Schema(new Uri(schemaLocationValue), targetNamespaceDeclaration);
                taxonomyFile.Add(schema);
            }
        }

        private IEnumerable<Uri> ExtractRelativeFilenames(XDocument xDocument, Uri parentFilename)
        {
            var schemaLocationAttributes = xDocument
                .Descendants(ImportXNames.Import)
                .SelectMany(e => e.Attributes())
                .Where(a => a.Name == ImportXNames.SchemaLocation);

            var schemaLocations = schemaLocationAttributes.Select(a => a.Value).ToList();

            var linkbaseRefAttributes = xDocument
                .Descendants(LinkbaseRefXNames.LinkbaseRef)
                .SelectMany(e => e.Attributes())
                .Where(a => a.Name == XLinkXNames.Href);

            var hrefs = linkbaseRefAttributes.Select(a => a.Value).ToList();

            var allRelativeLinks = schemaLocations.Union(hrefs).ToList();

            var parentDirectory = Path.GetDirectoryName(parentFilename.OriginalString) ?? string.Empty;
            return allRelativeLinks
                .Where(rl => !rl.StartsWith("http:"))
                .Select(rl => rl.Replace("/", @"\"))
                .Select(rl => Path.Combine(parentDirectory, rl))
                .Select(rl => new Uri(rl, UriKind.Relative));
        }

    }
}