using System.Collections.Generic;
using System.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Export.Writers
{
    public class ZipArchiveWriter : ITaxonomyWriter
    {
        /// <summary>
        /// The result of the write operation.
        /// </summary>
        public byte[] ZipArchiveBytes { get; set; }

        public void Write(Taxonomy taxonomy)
        {
            var taxonomyFiles = new HashSet<TaxonomyFile>();
            CollectFiles(taxonomy.EntryPoint, taxonomyFiles);

            var entries = taxonomyFiles.ToDictionary(tx => tx.FileName.OriginalString, tx => tx.ToXml().ToString());
            ZipArchiveBytes = ZipArchiveHelper.CreateFromEntries(entries);
        }

        private void CollectFiles(TaxonomyFile taxonomyFile, HashSet<TaxonomyFile> files)
        {
            // Only add taxonomy files with relative file names to the archive.
            if (!taxonomyFile.FileName.IsAbsoluteUri)
            {
                files.Add(taxonomyFile);
            }

            foreach (var dependency in taxonomyFile.Dependencies)
            {
                CollectFiles(dependency, files);
            }
        }
    }
}