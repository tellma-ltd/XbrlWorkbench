using System;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    internal class UnitAspect : Aspect
    {
        public UnitAspect(Unit unit):base(Dimension.UnitDimension)
        {
            Unit = unit;
        }

        public Unit Unit { get; }

        #region IEquatable

        public override bool Equals(Aspect other)
        {
            var typedOther = other as UnitAspect;
            return typedOther != null && typedOther.Unit == Unit;
        }

        public override int GetHashCode()
        {
            return Unit.Id.GetHashCode();
        }

        #endregion

        public override string ToString(IFormatProvider formatProvider)
        {
            return Unit.Id;
        }
    }
}