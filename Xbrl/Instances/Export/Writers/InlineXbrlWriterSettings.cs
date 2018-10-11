namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public class InlineXbrlWriterSettings : PresenterBasedWriterSettings
    {
        /// <summary>
        /// The content of the <title/> element in the <head/>.
        /// </summary>
        public string Title { get; set; }

        public bool IncludeReferences { get; set; }

        public bool IncludeDocumentation { get; set; }

        /// <summary>
        /// When set to true, all external references (CSS, JS) and all scripts are removed
        /// because no filing agency will allow these.
        /// </summary>
        public bool ForFiling { get; set; }
    }
}