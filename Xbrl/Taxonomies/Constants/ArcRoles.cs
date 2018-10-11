using System;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class ArcRoles
    {
        public static Uri ConceptToLabel = new Uri("http://www.xbrl.org/2003/arcrole/concept-label");
        public static Uri ParentToChild = new Uri("http://www.xbrl.org/2003/arcrole/parent-child");

        public static Uri PositiveHypercube = new Uri("http://xbrl.org/int/dim/arcrole/all");
        public static Uri HypercubeToDimension = new Uri("http://xbrl.org/int/dim/arcrole/hypercube-dimension");
        public static Uri DimensionToDomain = new Uri("http://xbrl.org/int/dim/arcrole/dimension-domain");
        public static Uri DomainToMember = new Uri("http://xbrl.org/int/dim/arcrole/domain-member");

        internal static Uri EssenceAlias = new Uri("http://www.xbrl.org/2003/arcrole/essence-alias");
        internal static Uri GeneralSpecial = new Uri("http://www.xbrl.org/2003/arcrole/general-special");
        internal static Uri SimilarTuples = new Uri("http://www.xbrl.org/2003/arcrole/similar-tuples");
        internal static Uri RequiresElement = new Uri("http://www.xbrl.org/2003/arcrole/requires-element");
        internal static Uri FactFootnote = new Uri("http://www.xbrl.org/2003/arcrole/fact-footnote");

    }
}