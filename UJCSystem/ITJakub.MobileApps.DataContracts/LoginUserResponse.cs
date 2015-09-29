using System;
using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public class LoginUserResponse
    {
        [DataMember]
        public long UserId { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public UserRoleContract UserRole { get; set; }

        [DataMember]
        public string CommunicationToken { get; set; }

        [DataMember]
        public DateTime EstimatedExpirationTime { get; set; }

        [DataMember]
        public string ProfilePictureUrl { get; set; }
    }
}