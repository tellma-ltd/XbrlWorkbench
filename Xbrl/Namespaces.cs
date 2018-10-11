using System.Xml.Linq;

namespace Banan.Tools.Xbrl
{
    public static class Namespaces
    {
        // XML

        public static XNamespace XLink = "http://www.w3.org/1999/xlink";
        public static XNamespace Xsd = "http://www.w3.org/2001/XMLSchema";
        public static XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";

        // XBRL

        public static XNamespace Xbrli = "http://www.xbrl.org/2003/instance";
        public static XNamespace Link = "http://www.xbrl.org/2003/linkbase";
        public static XNamespace Xbrldi = "http://xbrl.org/2005/xbrldi";
        public static XNamespace Xbrldt = "http://xbrl.org/2005/xbrldt";
        public static XNamespace Ref = "http://www.xbrl.org/2006/ref";
        public static XNamespace Iso4217 = "http://www.xbrl.org/2003/iso4217";
        public static XNamespace Num = "http://www.xbrl.org/dtr/type/numeric";
        public static XNamespace NonNum = "http://www.xbrl.org/dtr/type/non-numeric";
        public static XNamespace Label = "http://xbrl.org/2008/label";
        public static XNamespace Generic = "http://xbrl.org/2008/generic";
        public static XNamespace Reference = "http://xbrl.org/2008/reference"; // ??

        // iXBRL

        public static XNamespace Ix = "http://www.xbrl.org/2008/inlineXBRL";
        public static XNamespace Ixt = "http://www.xbrl.org/inlineXBRL/transformation/2010-04-20";
    }
}