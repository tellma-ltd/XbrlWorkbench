using System.Collections.Generic;

namespace Banan.Tools.XbrlBench.UI.Services
{
    public class DictionaryBackedState : IState
    {
        private readonly Dictionary<string,string> _dictionary;

        public DictionaryBackedState()
        {
            _dictionary = new Dictionary<string, string>();
        }

        public string GetString(string key)
        {
            return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
        }

        public void SetString(string key, string value)
        {
            _dictionary[key] = value;
        }

        public IEnumerable<string> Keys => _dictionary.Keys;
    }
}