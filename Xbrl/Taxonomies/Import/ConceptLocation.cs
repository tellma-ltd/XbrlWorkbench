using System;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class ConceptLocation : ItemLocation
    {
        public Uri NetworkRole { get; set; }

        public Uri PreferredLabelRole { get; set; }
    }
}