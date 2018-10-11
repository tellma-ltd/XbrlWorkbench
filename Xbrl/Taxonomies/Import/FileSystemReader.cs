using System.IO;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    public class FileSystemReader : IFileReader
    {
        private readonly string _basePath;

        public FileSystemReader(string basePath)
        {
            _basePath = basePath;
        }

        public XDocument Read(string uri)
        {
            var absolutePath = Path.Combine(_basePath, uri);
            return XDocument.Load(absolutePath);
        }
    }
}