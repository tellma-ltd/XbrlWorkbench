using System.Collections.Generic;
using Banan.Tools.Xbrl.Instances.Export.Presentation;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    internal class TableContext<TTable>
    {
        private Axis _horizontalAxis;

        public TTable Table { get; set; }

        public Axis HorizontalAxis
        {
            get { return _horizontalAxis; }
            set
            {
                _horizontalAxis = value;
                LinearisedHorizontalAxis = _horizontalAxis.Linearise();
            }
        }

        public IList<Member> LinearisedHorizontalAxis { get; private set; }
    }
}