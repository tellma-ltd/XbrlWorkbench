using System.Collections.Generic;

namespace Banan.Tools.XbrlBench.UI.Parsing
{
    public class ParsedCommand  
    {
        public ParsedCommand()
        {
            NamedParameters=new Dictionary<string, string>();
            PositionalParameters = new List<string>();
        }

        public string CommandName { get; set; }

        public IDictionary<string, string> NamedParameters { get; }

        public IList<string> PositionalParameters { get; }
    }
}