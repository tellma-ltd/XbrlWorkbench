using System.Collections.Generic;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class ExtensionMember : ExtensionItem
    {
        public ExtensionMember(string name) : base(name, DataTypeRegistry.DomainItem, null, PeriodTypes.Duration)
        {
            SubstitutionGroup = SubstitutionGroups.Item;
            IsAbstract = true;

            Locations=new List<MemberLocation>();
        }

        public IList<MemberLocation> Locations { get; set; }

        public void AddLocation(XName hypercubeName, XName dimensionName, XName parentName, XName precedingSiblingName)
        {
            var location = new MemberLocation
            {
                HypercubeName = hypercubeName,
                DimensionName = dimensionName,
                ParentName = parentName,
                PrecedingSiblingName = precedingSiblingName,
            };

            Locations.Add(location);
        }

    }
}