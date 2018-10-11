using System;

namespace Banan.Tools.Xbrl.Taxonomies.Constants
{
    public static class SchemaLocations
    {
        public static Uri Instance = new Uri("http://www.xbrl.org/2003/xbrl-instance-2003-12-31.xsd");
        public static Uri Num = new Uri("http://www.xbrl.org/dtr/type/numeric-2009-12-16.xsd");
        public static Uri NonNum = new Uri("http://www.xbrl.org/dtr/type/non-numeric");
        public static Uri Xbrldt = new Uri("http://www.xbrl.org/2005/xbrldt-2005.xsd");
    }
}