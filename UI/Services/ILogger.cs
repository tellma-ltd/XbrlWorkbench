using System.Collections.Generic;

namespace Banan.Tools.XbrlBench.UI.Services
{
    public interface ILogger
    {
        void WriteLine(string line);
    }

    public static class LoggerExtensions
    {
        public static void WriteLines(this ILogger logger, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                logger.WriteLine(line);
            }
        }
    }
}