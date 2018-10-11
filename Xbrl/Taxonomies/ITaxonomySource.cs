namespace Banan.Tools.Xbrl.Taxonomies
{
    public interface ITaxonomySource
    {
        /// <summary>
        /// Populates a taxonomy with schemata and linkbases.
        /// It is important that the taxonomy is already inside the DTS because of self-referencing locators.
        /// </summary>
        void Fill(Taxonomy taxonomy, DiscoverableTaxonomySet dts);
    }
}