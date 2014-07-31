using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    [KnownType(typeof(UserWithSalt))]
    public class User
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Email { get; set; }
    }

    [DataContract]
    public class UserWithSalt : User
    {
        [DataMember]
        public string Salt { get; set; }

        [DataMember]
        public string PasswordHash { get; set; }
    }
}