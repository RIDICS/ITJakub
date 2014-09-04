using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.Groups
{
    [DataContract]
    public class GroupDetailsUpdateContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public List<GroupMemberContract> Members { get; set; }
    }
}