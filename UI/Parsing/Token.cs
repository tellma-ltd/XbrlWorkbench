namespace Banan.Tools.XbrlBench.UI.Parsing
{
    internal class Token    
    {
        public string Text { get; }

        public Token(string text)
        {
            Text = text;
        }

        public bool IsPropertyName => Text.StartsWith("-");

        public string PropertyName => Text.TrimStart('-');
    }
}