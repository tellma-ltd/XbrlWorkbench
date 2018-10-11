using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public abstract class TaxonomyFile: ITaxonomyFileHolder
    {
        /// <summary>
        /// The file name is relative in most cases.
        /// The only exception are imported schemata that have absolute schema locations.
        /// </summary>
        public Uri FileName { get; }

        public IList<TaxonomyFile> Dependencies { get; }

        public Taxonomy Taxonomy { get; internal set; }

        protected TaxonomyFile(Uri fileName)
        {
            FileName = fileName;
            Dependencies = new List<TaxonomyFile>();
        }

        #region ITaxonomyFileHolder

        public void Add(TaxonomyFile taxonomyFile)
        {
            taxonomyFile.Taxonomy = Taxonomy;
            Dependencies.Add(taxonomyFile);
            Taxonomy.Dts.UpdateLookupStructures(taxonomyFile);
        }

        #endregion

        public abstract XDocument ToXml();

        protected IList<XElement> CreateImports()
        {
            var schemaDependencies = Dependencies.OfType<Schema>()
                .OrderByDescending(s => s.FileName.IsAbsoluteUri)
                .ToList();
            return schemaDependencies.Select(sd => sd.CreateImport()).ToList();
        }
    }
}