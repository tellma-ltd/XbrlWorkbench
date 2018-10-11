using System;
using Banan.Tools.Xbrl.Taxonomies.Constants;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public static class RelatedAspectRoles
    {
        public static Uri PeriodStart = LabelRoles.PeriodStart;
        public static Uri PeriodEnd = LabelRoles.PeriodEnd;

        // This URI is made up and no official XBRL URI.
        public static Uri Text = new Uri("http://www.xbrl.org/2003/instance/text");
    }
}