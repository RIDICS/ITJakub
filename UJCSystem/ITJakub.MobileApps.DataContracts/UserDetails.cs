using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserDetails
    {
        [DataMember]
        public long Id;

        [DataMember]
        public User User;

        [DataMember]
        public string Role;

        [DataMember]
        public List<long> GroupIds { get; set; }

        [DataMember]
        public List<long> TaskIds { get; set; }
    }
}