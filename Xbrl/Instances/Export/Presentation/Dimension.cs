using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    /// <summary>
    /// Dimensions are the domains that define the set of possible aspect values.
    /// XBRL knows four standard dimensions (entity, unit, period and concept).
    /// Hypercubes add further dimensions.
    /// </summary>
    /// <remarks>
    ///     Even though dimensions connect the FactSet data structure and the Axis data structure, they point to different
    ///     instances of Dimensions. This keeps the build-up of these two structures isolated, reflecting the separation
    ///     of XBRL taxonomy and XBRL instance.
    /// </remarks>
    public class Dimension
    {
        public Dimension(XName name)
        {
            Name = name;
        }

        /// <summary>
        ///     Built-in names for the four standard dimensions,
        ///     the dimension name for hypercube dimensions.
        /// </summary>
        public XName Name { get; }


        public static readonly Dimension EntityDimension = new Dimension(Namespaces.Xbrli + "EntityDimension");
        public static readonly Dimension UnitDimension = new Dimension(Namespaces.Xbrli + "UnitDimension");
        public static readonly Dimension PeriodDimension = new Dimension(Namespaces.Xbrli + "PeriodDimension");
        public static readonly Dimension ConceptDimension = new Dimension(Namespaces.Xbrli + "ConceptDimension");

    }
}