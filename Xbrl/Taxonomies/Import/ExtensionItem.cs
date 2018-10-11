using System;
using System.Collections.Generic;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public abstract class ExtensionItem
    {
        /// <summary>
        /// A simple string is enough because the namespace and the prefix are defined
        /// for all items by the TargetNamespaceDeclaration in the TaxonomyExtension. 
        /// </summary>
        public string Name { get; }

        public string BalanceType { get; }

        public string DataType { get; }

        public string PeriodType { get; }

        internal protected string SubstitutionGroup { get; set; }

        internal protected bool IsAbstract { get; set; }


        public IList<ExtensionLabel> Labels { get; }

        protected ExtensionItem(string name, string dataType, string balanceType, string periodType)
        {
            Name = name;
            DataType = dataType;
            BalanceType = balanceType;
            PeriodType = periodType;

            Labels = new List<ExtensionLabel>();
        }

        public void AddLabel(string text, string language)
        {
            Labels.Add(new ExtensionLabel(LabelRoles.Standard, text, language));
        }
    }

    public class ExtensionLabel
    {
        public Uri Role { get; }

        public string Language { get; }

        public string Text { get; }

        public ExtensionLabel(Uri role, string text, string language)
        {
            Role = role;
            Text = text;
            Language = language;
        }
    }
}