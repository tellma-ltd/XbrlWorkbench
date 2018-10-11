using System;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <remarks>
    /// There is intentionally no link to a Concept instance to align the aspects data structure with the XBRL instance.
    /// </remarks>
    internal class ConceptAspect : Aspect
    {
        public XName ConceptName { get; }

        public ConceptAspect(XName conceptName) : base(Dimension.ConceptDimension)
        {
            ConceptName = conceptName;
        }

        #region IEquatable

        public override bool Equals(Aspect other)
        {
            var typedOther = other as ConceptAspect;
            return typedOther != null && typedOther.ConceptName == ConceptName;
        }

        public override int GetHashCode()
        {
            return ConceptName.GetHashCode() ^ Dimension.Name.GetHashCode();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            // Concepts are rendered by the ConceptMember.
            throw new NotImplementedException();
        }

        #endregion

    }
}