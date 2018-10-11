using System;
using System.IO.Compression;
using Banan.Tools.Xbrl.Instances;
using Banan.Tools.Xbrl.Taxonomies.Import;
using Unity.Attributes;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Add-BaseTaxonomy", CommandType = typeof(AddBaseTaxonomyCommand))]
    public class AddBaseTaxonomyCommand : ShellCommandBase
    {
        [Dependency]
        public Instance Instance { get; set; }


        public override void Invoke()
        {
            base.Invoke();

            if (!NamedParameters.ContainsKey("entry"))
            {
                Context.Logger.WriteLine("Parameter 'entry' is missing.");
                return;
            }
            var entry = NamedParameters["entry"];

            IFileReader fileReader;

            if (NamedParameters.ContainsKey("manifest"))
            {
                var manifest = NamedParameters["manifest"];
                var manifestReader = new ManifestResourcesReader(GetType());
                var stream = manifestReader.GetStream(manifest);
                var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
                fileReader = new ZipArchiveReader(zipArchive);
            } else
            if (NamedParameters.ContainsKey("archive"))
            {
                var archive = NamedParameters["archive"];
                var zipArchive = ZipFile.Open(archive, ZipArchiveMode.Read);
                fileReader = new ZipArchiveReader(zipArchive);
            }
            else if (NamedParameters.ContainsKey("dir"))
            {
                var dir = NamedParameters["dir"];
                fileReader = new FileSystemReader(dir);
            }
            else
            {
                Context.Logger.WriteLine("You must specify either dir, manifest or archive to define the source location of the entry point.");
                return;
            }

            var taxonomySource = new TaxonomyFileSet(fileReader);
            Instance.Dts.AddTaxonomy(new Uri(entry), taxonomySource);
        }

    }
}