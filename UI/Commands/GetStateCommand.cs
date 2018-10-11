using System.Linq;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Get-State", CommandType = typeof(GetStateCommand))]
    public class GetStateCommand : ShellCommandBase
    {
        public override void Invoke()
        {
            base.Invoke();

            var keys = NamedParameters.Any() ? NamedParameters.Keys : Context.State.Keys;



            foreach (var key in keys)
            {
                Context.Logger.WriteLine($"{key}: {Context.State.GetString(key)}");
            }
        }
    }
}