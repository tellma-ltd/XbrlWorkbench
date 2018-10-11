using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Banan.Tools.Xbrl.Taxonomies.Export.Writers
{
    public static class ZipArchiveHelper {

        public static IDictionary<string, string> ExtractEntries(byte[] zipArchiveBytes)
        {
            var entries = new Dictionary<string,string>();
            using (var archiveMemoryStream = new MemoryStream(zipArchiveBytes))
            {
                using (var zipArchive = new ZipArchive(archiveMemoryStream, ZipArchiveMode.Read, false))
                {
                    foreach (var zipArchiveEntry in zipArchive.Entries)
                    {
                        using (var entryStream = zipArchiveEntry.Open())
                        {
                            using (var reader = new StreamReader(entryStream))
                            {
                                entries[zipArchiveEntry.FullName] = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            return entries;
        }

        public static byte[] CreateFromEntries(IDictionary<string, string> entries)
        {
            byte[] result;
            using (var archiveMemoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(archiveMemoryStream, ZipArchiveMode.Create, false))
                {
                    foreach (var entry in entries)
                    {
                        var zipArchiveEntry = zipArchive.CreateEntry(entry.Key);
                        using (var entryStream = zipArchiveEntry.Open())
                        {
                            using (var writer = new StreamWriter(entryStream))
                            {
                                writer.Write(entry.Value);
                            }
                        }
                    }
                }

                result = archiveMemoryStream.ToArray();
            }
            return result;
        }
    }
}