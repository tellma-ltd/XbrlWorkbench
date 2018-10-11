using System;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    internal class PeriodAspect : Aspect
    {
        public PeriodAspect(Period period) : base(Dimension.PeriodDimension)
        {
            Period = period;
        }

        public Period Period { get; }

        #region IEquatable

        public override bool Equals(Aspect other)
        {
            var typedOther = other as PeriodAspect;
            return typedOther != null && typedOther.Period.Id == Period.Id;
        }

        public override int GetHashCode()
        {
            return Period.Id.GetHashCode();
        }

        #endregion

        public override string ToString(IFormatProvider formatProvider)
        {
            var durationPeriod = Period as DurationPeriod;
            if (durationPeriod!=null)
            {
                return VerboseDurationPeriod(durationPeriod, formatProvider);
            }

            var instantPeriod = Period as InstantPeriod;
            if (instantPeriod != null)
            {
                return VerboseInstantPeriod(instantPeriod, formatProvider);
            }

            return "Unsupported period type";
        }

        private string VerboseInstantPeriod(InstantPeriod instantPeriod, IFormatProvider formatProvider)
        {
            return instantPeriod.Date.ToString("d", formatProvider);
        }

        private string VerboseDurationPeriod(DurationPeriod durationPeriod, IFormatProvider formatProvider)
        {
            if (durationPeriod.StartDate.Year == durationPeriod.EndDate.Year
                && durationPeriod.StartDate.Day == 1 
                && durationPeriod.StartDate.Month == 1 
                && durationPeriod.EndDate.Day == 31 
                && durationPeriod.EndDate.Month == 12)
            {
                return durationPeriod.StartDate.Year.ToString();
            }

            return string.Format(formatProvider, "{0:d} - {1:d}", durationPeriod.StartDate, durationPeriod.EndDate);
        }
    }
}