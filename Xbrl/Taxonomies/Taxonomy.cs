using System;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class Taxonomy: ITaxonomyFileHolder
    {
        public Uri EntryPointUri { get; }

        public TaxonomyFile EntryPoint { get; private set; }

        public DiscoverableTaxonomySet Dts { get; }

        internal Taxonomy(Uri entryPointUri, DiscoverableTaxonomySet discoverableTaxonomySet)
        {
            EntryPointUri = entryPointUri;
            Dts = discoverableTaxonomySet;
        }

        public void SetEntryPoint(TaxonomyFile entryPoint)
        {
            EntryPoint = entryPoint;
            entryPoint.Taxonomy = this;
            Dts.UpdateLookupStructures(entryPoint);
        }

        #region ITaxonomyFileHolder

        public void Add(TaxonomyFile taxonomyFile)
        {
            if (EntryPoint != null)
            {
                throw new InvalidOperationException("Tried to add a second entry point to a taxonomy.");
            }
            SetEntryPoint(taxonomyFile);
        }

        #endregion

        public void Save(ITaxonomyWriter writer)
        {
            writer.Write(this);
        }

    }
}