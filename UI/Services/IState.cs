using System.Collections.Generic;

namespace Banan.Tools.XbrlBench.UI.Services
{
    public interface IState
    {
        string GetString(string key);

        void SetString(string key, string value);

        IEnumerable<string> Keys { get; }

    }
}