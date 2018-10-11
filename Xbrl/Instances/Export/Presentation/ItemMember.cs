using System;
using Banan.Tools.Xbrl.Taxonomies;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public abstract class ItemMember : Member
    {
        public string Label { get; set; }

        public Item Item { get; set; }

        protected ItemMember(Aspect aspect) : base(aspect)
        {
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return Label;
        }
    }
}