using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserDetails
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public User User { get; set; }
    }
}