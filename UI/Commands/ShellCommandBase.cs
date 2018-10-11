using System.Collections.Generic;
using Unity.Attributes;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    public abstract class ShellCommandBase : IShellCommand
    {
        [Dependency]
        public CommandContext Context { get; set; }

        [Dependency("NamedParameters")]
        public IDictionary<string, string> NamedParameters { get; set; }

        [Dependency("PositionalParameters")]
        public IList<string> PositionalParameters { get; set; }

        public virtual void Invoke()
        {
            
        }
    }
}