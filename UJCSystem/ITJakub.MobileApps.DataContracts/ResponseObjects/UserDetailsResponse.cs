using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.ResponseObjects
{
    [DataContract]
    public class UserDetailsResponse
    {
        [DataMember]
        public string FirstName;

        [DataMember]
        public string LastName;

        [DataMember]
        public string Email;

        [DataMember]
        public string Role;

    }
}