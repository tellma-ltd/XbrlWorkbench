using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class ExtensionConcept : ExtensionItem
    {
        public ExtensionConcept(string name, string dataType, string balanceType, string periodType) : base(name, dataType, balanceType, periodType)
        {
            SubstitutionGroup = SubstitutionGroups.Item;
            IsAbstract = false;

            Locations = new List<ConceptLocation>();
        }

        public IList<ConceptLocation> Locations { get; set; }

        public void AddLocation(Uri networkRole, XName parentName, XName precedingSiblingName)
        {
            var location = new ConceptLocation
            {
                NetworkRole = networkRole,
                ParentName = parentName,
                PrecedingSiblingName = precedingSiblingName,
                PreferredLabelRole = LabelRoles.Standard
            };

            Locations.Add(location);
        }
    }
}