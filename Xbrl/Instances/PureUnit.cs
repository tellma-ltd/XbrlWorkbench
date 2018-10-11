using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies;

namespace Banan.Tools.Xbrl.Instances
{
    public class PureUnit : Unit
    {
        public PureUnit()
        {
            Id = "pure";
        }

        public override XElement ToXml()
        {
            var unitElement = base.ToXml();
            unitElement.Add(
                new XAttribute("id", "pure"),
                new XElement(Namespaces.Xbrli + "measure", "xbrli:pure"));
            return unitElement;
        }

    }
}