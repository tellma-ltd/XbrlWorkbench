using System.Collections.Generic;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Export.Writers;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public class InlineXbrlFilingWriter : IInstanceWriter
    {
        private readonly XDocument _template;
        private readonly InlineXbrlFilingWriterSettings _settings;

        /// <summary>
        /// The result of the write operation.
        /// </summary>
        public byte[] ZipArchiveBytes { get; set; }

        public InlineXbrlFilingWriter(XDocument template, InlineXbrlFilingWriterSettings settings)
        {
            _template = template;
            _settings = settings;

            _settings.ForFiling = true;
        }

        #region IInstanceWriter

        public void Write(Instance instance)
        {
            var entries = new Dictionary<string, string>();

            var inlineXbrlWriter = new InlineXbrlWriter(_template, _settings);
            instance.Save(inlineXbrlWriter);
            entries[_settings.InlineXbrlFileName] = inlineXbrlWriter.Document.ToString();

            foreach (var taxonomy in instance.Dts.Taxonomies)
            {
                if (taxonomy.EntryPointUri.IsAbsoluteUri)
                {
                    // No need to include taxonomies with absolute entry points in the archive.
                    continue;
                }

                var taxonomyWriter = new ZipArchiveWriter();
                taxonomy.Save(taxonomyWriter);
                var taxonmyEntries = ZipArchiveHelper.ExtractEntries(taxonomyWriter.ZipArchiveBytes);
                foreach (var taxonmyEntry in taxonmyEntries)
                {
                    entries[taxonmyEntry.Key] = taxonmyEntry.Value;
                }
            }

            ZipArchiveBytes = ZipArchiveHelper.CreateFromEntries(entries);
        }

        #endregion

    }
}