using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances
{
    public abstract class Unit
    {
        public string Id { get; protected set; }

        public virtual XElement ToXml()
        {
            return new XElement(Namespaces.Xbrli + "unit");
        }
    }
}