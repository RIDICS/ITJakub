using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    [KnownType(typeof(OwnedDetailGroupContract))]
    public class GroupDetailContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public List<GroupMemberContract> Members { get; set; }
    }

    [DataContract]
    public class OwnedDetailGroupContract : GroupDetailContract
    {
        [DataMember]
        public string EnterCode { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }
}