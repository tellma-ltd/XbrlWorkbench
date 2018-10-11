using Banan.Tools.XbrlBench.UI.Services;
using Unity.Attributes;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    public class CommandContext
    {
        [Dependency]
        public ILogger Logger { get; set; }


        [Dependency]
        public IState State { get; set; }
    }
}