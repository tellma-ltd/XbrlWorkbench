using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Taxonomies.Import
{
    /// <summary>
    /// This interface abstracts away the physicality of a file and
    /// allows reading from either the file system or from a zip archive.
    /// </summary>
    public interface IFileReader
    {
        XDocument Read(string uri);
    }
}