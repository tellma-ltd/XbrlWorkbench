using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances
{
    public class ForeverPeriod : Period
    {
        public override string Id => "forever";

        public override string PeriodType => PeriodTypes.Duration;

        public override XElement ToXml()
        {
            var periodElement = base.ToXml();
            periodElement.Add(
                new XElement(Namespaces.Xbrli + "forever"));
            return periodElement;
        }

    }
}