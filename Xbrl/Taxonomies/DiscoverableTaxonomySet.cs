using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies
{
    public class DiscoverableTaxonomySet
    {
        public DiscoverableTaxonomySet()
        {
            Taxonomies = new HashSet<Taxonomy>();

            _itemsById = new Dictionary<string, Item>();
            _itemsByName = new Dictionary<XName, Item>();
            _roleTypesByRole = new Dictionary<Uri, RoleType>();
            _linksByRole = new Dictionary<Uri, Link>();
        }

        public ISet<Taxonomy> Taxonomies { get; }


        public void UpdateLookupStructures(TaxonomyFile taxonomyFile)
        {
            var schema = taxonomyFile as Schema;
            if (schema != null)
            {
                foreach (var item in schema.Items)
                {
                    _itemsById[item.Id] = item;
                    _itemsByName[item.Name] = item;
                }

                foreach (var roleType in schema.RoleTypes)
                {
                    _roleTypesByRole[roleType.Role] = roleType;
                }
            }

            var linkbase = taxonomyFile as Linkbase;
            if (linkbase != null)
            {
                foreach (var link in linkbase.Links)
                {
                    _linksByRole[link.Role] = link;
                }
            }
        }

        public Taxonomy AddTaxonomy(Uri entryPoint, ITaxonomySource source)
        {
            var taxonomy = new Taxonomy(entryPoint, this);
            Taxonomies.Add(taxonomy);

            source.Fill(taxonomy, this);

            return taxonomy;
        }

        /// <summary>
        ///     Finds an item by its id. To be 100% precise, one would need to consider the entire reference including the
        ///     schema path and name. Searching by id alone sufficient because it is customary to include the namespace prefix in
        ///     the id.
        /// </summary>
        public Item FindItem(string id)
        {
            Item item;
            _itemsById.TryGetValue(id, out item);
            return item;
        }

        public Item FindItem(XName name)
        {
            Item item;
            _itemsByName.TryGetValue(name, out item);
            return item;
        }

        public PresentationLink FindPresentationLink(Uri role)
        {
            Link link;
            _linksByRole.TryGetValue(role, out link);
            return link as PresentationLink;
        }

        public IEnumerable<Link> GetAllLinks()
        {
            return _linksByRole.Values;
        }

        public void AddItemNamespaceDeclarations(XElement holder)
        {
            var distinctSchemata = _itemsByName.Values
                .Select(i => i.Schema)
                .Distinct();

            foreach (var schema in distinctSchemata)
            {
                holder.Add(schema.TargetNamespaceDeclaration);
            }
        }

        public RoleType FindRoleType(Uri role)
        {
            RoleType roleType;
            _roleTypesByRole.TryGetValue(role, out roleType);
            return roleType;
        }


        #region Cross-taxonomy data structures for super fast lookups. 

        private readonly IDictionary<string, Item> _itemsById;

        private readonly IDictionary<XName, Item> _itemsByName;

        private readonly IDictionary<Uri, RoleType> _roleTypesByRole;

        private readonly IDictionary<Uri, Link> _linksByRole;

        #endregion
    }
}