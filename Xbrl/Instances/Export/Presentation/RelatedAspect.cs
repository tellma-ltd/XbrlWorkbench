using System;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public class RelatedAspect
    {
        public Aspect Aspect { get; }

        public Uri Role { get; }

        public RelatedAspect(Aspect aspect, Uri role)
        {
            Aspect = aspect;
            Role = role;
        }
    }
}