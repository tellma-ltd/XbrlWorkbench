using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class ExtensionAbstractConcept : ExtensionConcept
    {
        public ExtensionAbstractConcept(string name) : base(name, DataTypeRegistry.String, null, PeriodTypes.Duration)
        {
            IsAbstract = true;
        }
    }
}