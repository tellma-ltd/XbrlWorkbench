using System.Linq;
using Banan.Tools.XbrlBench.UI.Services;
using Unity.Attributes;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Get-Command", CommandType = typeof(GetCommandCommand))]
    public class GetCommandCommand : ShellCommandBase
    {
        [Dependency]
        public ICommandRegistry CommandRegistry { get; set; }

        public override void Invoke()
        {
            base.Invoke();

            var names = CommandRegistry.GetNames();

            foreach (var nameGroup in names.GroupBy(GetNoun).OrderBy(g => g.Key))
            {
                var line = string.Join(", ", nameGroup.OrderBy(n => n));
                Context.Logger.WriteLine(line);
            }
        }

        private string GetNoun(string commandName)
        {
            var parts = commandName.Split('-');
            return parts[1];
        }
    }
}