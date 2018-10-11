using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Linq;
using Banan.Tools.XbrlBench.UI.Commands;

namespace Banan.Tools.XbrlBench.UI.Services
{
    public class Mef2BasedCommandRegistry : ICommandRegistry
    {
        private IDictionary<string, Type> _commandExports;

        public void Discover()
        {
            var configuration = new ContainerConfiguration()
                .WithAssembly(GetType().Assembly);
            var host = configuration.CreateContainer();
            var parts = host.GetExports<ExportFactory<ShellCommandBase, CommandExportAttribute>>();

            _commandExports = parts.ToDictionary(part => part.Metadata.Name, part => part.Metadata.CommandType, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<string> GetNames()
        {
            return _commandExports.Select(ce => ce.Key);
        }

        public Type FindType(string name)
        {
            Type commandType;
            _commandExports.TryGetValue(name, out commandType);
            return commandType;
        }
    }
}