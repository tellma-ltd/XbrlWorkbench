using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances
{
    /// <summary>
    /// The XBRL specifications say that the XML Schema data type for the Date property can be date or dateTime,
    /// that means with or without a time component. This implementation treats the Date property as date only as this seems to be the
    /// common practice in filings.
    /// </summary>
    public class InstantPeriod : Period
    {
        public DateTime Date { get; }

        public override string Id => $"{Date.Day}-{Date.Month}-{Date.Year}";

        public override string PeriodType => PeriodTypes.Instant;

        public InstantPeriod(DateTime date)
        {
            Date = date;
        }

        public override XElement ToXml()
        {
            var periodElement = base.ToXml();
            periodElement.Add(
                new XElement(Namespaces.Xbrli + "instant", Date.ToString("yyyy-MM-dd")));
            return periodElement;
        }

    }
}