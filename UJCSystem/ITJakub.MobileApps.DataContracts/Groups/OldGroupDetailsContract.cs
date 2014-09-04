using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    public class OldGroupDetailsContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public IList<long> MemberIds { get; set; }
    }
}