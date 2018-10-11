using System;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <summary>
    /// An aspect is a characteristic that qualifies a fact, e.g. the entity, the unit, the concept, an axis member, etc.
    /// </summary>
    /// <remarks>
    /// The term "aspect" is borrowed from the fantastic book "The XBRL Book" which proved invaluable to develop this code.
    /// </remarks>
    public abstract class Aspect : IEquatable<Aspect>
    {
        protected Aspect(Dimension dimension)
        {
            Dimension = dimension;
        }

        public Dimension Dimension { get; }

        #region IEquatable

        public abstract bool Equals(Aspect other);

        public abstract override int GetHashCode();

        #endregion

        public abstract string ToString(IFormatProvider formatProvider);
    }
}