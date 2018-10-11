using System;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    internal class EntityAspect : Aspect
    {
        public Entity Entity { get; }

        public EntityAspect(Entity entity):base(Dimension.EntityDimension)
        {
            Entity = entity;
        }

        #region IEquatable

        public override bool Equals(Aspect other)
        {
            var typedOther = other as EntityAspect;
            return typedOther != null && typedOther.Entity == Entity;
        }

        public override int GetHashCode()
        {
            return Entity.IdentifierScheme.GetHashCode() ^ Dimension.Name.GetHashCode();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return Entity.Name;
        }

        #endregion

        public override string ToString()
        {
            return Entity.Name;
        }
    }
}