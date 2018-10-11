using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Banan.Tools.Xbrl.Instances.Export.Writers
{
    [DataContract]
    internal class ItemMemberInfo
    {
        [DataMember]
        internal string name;

        [DataMember]
        internal string doc;

        [DataMember]
        internal IList<ReferenceInfo> refs;
    }

    [DataContract]
    internal class ReferenceInfo
    {
        [DataMember]
        internal string loc;

        [DataMember]
        internal string type;

        [DataMember]
        internal Uri link;
    }

}