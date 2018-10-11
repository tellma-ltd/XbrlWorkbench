using System.Collections.Generic;
using Banan.Tools.Xbrl.Taxonomies;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    internal class Hypercube
    {
        public Hypercube(Item hypercubeItem)
        {
            HypercubeItem = hypercubeItem;
            Axes = new List<Axis>();
        }

        public Item HypercubeItem { get; }

        public IList<Axis> Axes { get; set; }
    }
}