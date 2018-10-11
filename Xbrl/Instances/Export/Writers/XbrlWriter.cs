using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public class XbrlWriter : IInstanceWriter
    {
        /// <summary>
        /// The result of the write operation.
        /// </summary>
        public XDocument Document { get; private set; }


        #region IInstanceWriter

        public void Write(Instance instance)
        {
            Document = instance.ToXml();
        }

        #endregion

    }
}