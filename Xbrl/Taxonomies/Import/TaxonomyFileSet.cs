using System;
using System.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
        /// <summary>
        /// Represents a file-based taxonomy.
        /// </summary>
    public class TaxonomyFileSet : ITaxonomySource
    {
        private readonly IFileReader _fileReader;

        public TaxonomyFileSet(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public void Fill(Taxonomy taxonomy, DiscoverableTaxonomySet dts)
        {
            var taxonomyDagProcessor = new TaxonomyDagProcessor(dts, _fileReader);

            var entryFileName = new Uri(taxonomy.EntryPointUri.Segments.Last(), UriKind.Relative);
            taxonomyDagProcessor.Process(entryFileName, taxonomy);
        }

    }
}