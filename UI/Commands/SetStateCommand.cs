namespace Banan.Tools.XbrlBench.UI.Commands
{
    [CommandExport(Name = "Set-State", CommandType = typeof(SetStateCommand))]
    public class SetStateCommand : ShellCommandBase
    {
        public override void Invoke()
        {
            base.Invoke();

            foreach (var namedParameter in NamedParameters)
            {
                Context.State.SetString(namedParameter.Key, namedParameter.Value);
            }
        }
    }
}