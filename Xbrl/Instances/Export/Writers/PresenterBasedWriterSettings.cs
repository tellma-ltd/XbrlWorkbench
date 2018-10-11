using System.Globalization;
using Banan.Tools.Xbrl.Instances.Export.Presentation;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public abstract class PresenterBasedWriterSettings
    {
        /// <summary>
        /// These settings are passed into the presenter.
        /// </summary>
        public ScopeSettings Scope { get; set; }

        /// <summary>
        ///     The culture defines the rendering of numerical values, in particular the group separator, the decimal separator
        ///     and the number of decimals.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        ///     In powers of ten, i.e. 3 for a scaling of 1000.
        /// </summary>
        public int Scale { get; set; }
    }
}