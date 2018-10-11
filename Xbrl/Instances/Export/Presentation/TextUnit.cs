using System;
using System.Xml.Linq;

namespace Banan.Tools.Xbrl.Instances.Export.Presentation
{
    internal class TextUnit : Unit
    {
        public static TextUnit Instance = new TextUnit();

        public TextUnit()
        {
            Id = "text";
        }

        public override XElement ToXml()
        {
            throw new NotImplementedException("TextUnit lives behind the scenes and is never exported.");
        }
    }
}