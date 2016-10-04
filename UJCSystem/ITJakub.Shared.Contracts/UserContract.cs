using System;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(UserDetailContract))]
    [KnownType(typeof(PrivateUserContract))]
    public class UserContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }
    }

    [DataContract]
    public class PrivateUserContract : UserContract
    {
        [DataMember]
        public string PasswordHash { get; set; }

        [DataMember]
        public string CommunicationToken { get; set; }

        [DataMember]
        public DateTime CommunicationTokenExpirationTime { get; set; }
    }
}