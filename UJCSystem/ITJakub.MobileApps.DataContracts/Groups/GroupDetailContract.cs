using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    [KnownType(typeof(OwnedGroupContract))]
    public class GroupDetailContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsActive { get; set; }
    }

    [DataContract]
    public class OwnedGroupContract : GroupDetailContract
    {
        [DataMember]
        public string EnterCode { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }
}