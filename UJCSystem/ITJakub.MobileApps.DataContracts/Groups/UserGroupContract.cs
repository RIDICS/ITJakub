using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    public class UserGroupsContract
    {
        [DataMember]
        public List<GroupDetailContract> MemberOfGroup { get; set; }

        [DataMember]
        public List<OwnedDetailGroupContract> OwnedGroups { get; set; }
    }
}