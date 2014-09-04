using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ITJakub.MobileApps.DataContracts.Tasks;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    [KnownType(typeof(OwnedGroupInfoContract))]
    [KnownType(typeof(GroupDetailContract))]
    public class GroupInfoContract
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
    public class OwnedGroupInfoContract : GroupInfoContract
    {
        [DataMember]
        public string EnterCode { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }

    [DataContract]
    public class GroupDetailContract : OwnedGroupInfoContract
    {
        [DataMember]
        public TaskDetailContract Task { get; set; }
    }
}