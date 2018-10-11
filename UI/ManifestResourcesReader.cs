using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Banan.Tools.XbrlBench.UI
{
    public class ManifestResourcesReader
    {
        private readonly Assembly _assembly;

        public ManifestResourcesReader(Assembly assembly)
        {
            _assembly = assembly;
        }

        public ManifestResourcesReader(Type markerType) : this(markerType.Assembly)
        {
        }

        public string GetString(string resourceName)
        {
            using (var stream = GetStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public Stream GetStream(string resourceName)
        {
            var allNames = _assembly.GetManifestResourceNames();

            var match = allNames.FirstOrDefault(n => n.Contains(resourceName));
            if (match == null)
            {
                throw new ArgumentException($"No resource found with the name '{resourceName}'. Make sure the build action for the file is set to 'Embedded resource'.");
            }

            return _assembly.GetManifestResourceStream(match);
        }

        public byte[] GetBytes(string resourceName)
        {
            using (var stream = GetStream(resourceName))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public string[] GetNames(Regex pattern)
        {
            var allNames = _assembly.GetManifestResourceNames();
            var matchingNames = allNames.Where(n => pattern.IsMatch(n)).ToArray();
            return matchingNames;
        }

    }
}