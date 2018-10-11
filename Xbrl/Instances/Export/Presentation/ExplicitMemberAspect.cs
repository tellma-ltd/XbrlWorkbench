using System;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    internal class ExplicitMemberAspect : Aspect
    {
        public XName MemberName { get; }

        public ExplicitMemberAspect(Dimension dimension, XName memberName) : base(dimension)
        {
            MemberName = memberName;
        }

        #region IEquatable

        public override bool Equals(Aspect other)
        {
            var typedOther = other as ExplicitMemberAspect;
            return typedOther != null && typedOther.Dimension.Name == Dimension.Name && typedOther.MemberName == MemberName;
        }

        public override int GetHashCode()
        {
            return MemberName.GetHashCode() ^ Dimension.Name.GetHashCode();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}