using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances
{
    public abstract class Period
    {
        /// <summary>
        /// This Id only serves to generate a unique context id. It is not part of the XBRL class model.
        /// </summary>
        public abstract string Id { get; }

        public abstract string PeriodType { get; }

        public virtual XElement ToXml()
        {
            return new XElement(Namespaces.Xbrli + "period");
        }
    }
}