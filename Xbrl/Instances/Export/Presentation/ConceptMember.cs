using System;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    public class ConceptMember : ItemMember
    {
        public Uri PreferredLabelRole { get; set; }

        /// <summary>
        /// Hypercube axes can branch of concept members.
        /// </summary>
        /// <remarks>
        /// In XBRL, a hypercube contains a list of axes that all span the *same* cube.
        /// This code treats them instead as a list of cubes (tables) with one axis each.
        /// </remarks>
        public Axis HypercubeAxis { get; set; }

        public ConceptMember(Aspect aspect) : base(aspect)
        {
        }
    }
}