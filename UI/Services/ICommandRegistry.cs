using System;
using System.Collections.Generic;

namespace Banan.Tools.XbrlBench.UI.Services
{
    public interface ICommandRegistry
    {
        IEnumerable<string> GetNames();

        Type FindType(string name);
    }
}