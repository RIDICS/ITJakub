using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    public class UserGroupsContract
    {
        [DataMember]
        public List<GroupInfoContract> MemberOfGroup { get; set; }

        [DataMember]
        public List<OwnedGroupInfoContract> OwnedGroups { get; set; }
    }
}