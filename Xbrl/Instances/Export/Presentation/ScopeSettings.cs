using System.Collections.Generic;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public class ScopeSettings
    {
        /// <summary>
        ///     Defines which presentation networks to include and in what order.
        /// </summary>
        public IList<PresentationNetwork> PresentationNetworks { get; set; }

        /// <summary>
        /// If true, renders all concepts, even when facts are absent for some concepts.
        /// Default: false
        /// </summary>
        public bool IncludeEmptyConcepts { get; set; }

        /// <summary>
        /// If true, renders all axis members, even when facts are absent for some members.
        /// Default: false
        /// </summary>
        public bool IncludeEmptyExplicitMembers { get; set; }
    }
}