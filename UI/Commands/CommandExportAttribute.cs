using System;
using System.Composition;

namespace Banan.Tools.XbrlBench.UI.Commands
{
    [MetadataAttribute]
    public class CommandExportAttribute : ExportAttribute
    {
        public CommandExportAttribute() : base(typeof(ShellCommandBase))
        {

        }

        public string Name { get; set; }

        public Type CommandType { get; set; }
    }
}