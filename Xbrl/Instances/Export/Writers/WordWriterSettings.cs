namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    public class WordWriterSettings : PresenterBasedWriterSettings
    {
        public string PlaceholderPrefix { get; set; } = "{";

        public string PlaceholderSuffix { get; set; } = "}";
    }
}