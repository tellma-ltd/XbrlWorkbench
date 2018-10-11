using System;
using System.Xml.Linq;
using Banan.Tools.Xbrl.Taxonomies;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances
{
    /// <summary>
    /// The XBRL specifications say that the XML Schema data type for start and end dates can be date or dateTime,
    /// that means with or without a time component. This implementation treats them as date only as this seems to be the
    /// common practice in filings.
    /// </summary>
    public class DurationPeriod : Period
    {
        public DurationPeriod(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// From the XBRL specification:
        /// "defined to be equivalent to specifying a dateTime of the same date, and T00:00:00 (midnight at the start of the day)".
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// From the XBRL specification:
       /// "defined to be equivalent to specifying a dateTime of the same date plus P1D and with a time part of T00:00:00.
        /// This represents midnight at the end of the day."
        /// </summary>
        public DateTime EndDate { get; }

        public override string Id => $"{StartDate.Day}-{StartDate.Month}-{StartDate.Year}_{EndDate.Day}-{EndDate.Month}-{EndDate.Year}";

        public override string PeriodType => PeriodTypes.Duration;


        public override XElement ToXml()
        {
            var periodElement = base.ToXml();
            periodElement.Add(
                new XElement(Namespaces.Xbrli + "startDate", StartDate.ToString("yyyy-MM-dd")),
                new XElement(Namespaces.Xbrli + "endDate", EndDate.ToString("yyyy-MM-dd")));
            return periodElement;
        }
    }
}