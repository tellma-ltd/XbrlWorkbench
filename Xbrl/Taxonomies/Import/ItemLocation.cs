using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public abstract class ItemLocation
    {
        public XName ParentName { get; set; }

        public XName PrecedingSiblingName { get; set; }
    }
}