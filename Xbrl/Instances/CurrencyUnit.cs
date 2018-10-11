using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances
{
    public class CurrencyUnit : Unit
    {
        public CurrencyUnit(string iso4217Code)
        {
            Id = iso4217Code;
            Iso4217Code = iso4217Code;
        }

        public string Iso4217Code { get; }

        public override XElement ToXml()
        {
            var unitElement = base.ToXml();
            unitElement.Add(
                new XAttribute("id", Iso4217Code),
                new XElement(Namespaces.Xbrli + "measure", $"iso4217:{Iso4217Code}"));
            return unitElement;
        }

    }
}