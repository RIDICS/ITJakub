using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts.RequestObjects
{
    [DataContract]
    public class User
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