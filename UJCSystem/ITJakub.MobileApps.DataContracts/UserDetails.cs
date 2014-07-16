using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class UserDetails
    {
        [DataMember]
        public User User;

        [DataMember]
        public string Role;
    }
}