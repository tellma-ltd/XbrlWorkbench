using System.IO.Compression;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class ZipArchiveReader : IFileReader
    {
        private readonly ZipArchive _zipArchive;

        public ZipArchiveReader(ZipArchive zipArchive)
        {
            _zipArchive = zipArchive;
        }

        public XDocument Read(string uri)
        {
            var sanitisedUri = uri.Replace(@"\", "/");
            var entry = _zipArchive.GetEntry(sanitisedUri);
            if (entry==null)
            {
                return null;
            }

            using (var stream = entry.Open())
            {
                return XDocument.Load(stream);
            }
        }
    }
}