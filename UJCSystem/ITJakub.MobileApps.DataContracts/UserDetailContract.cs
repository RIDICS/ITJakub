using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    [KnownType(typeof (PasswordUserDetailContract))]
    [KnownType(typeof (GroupMemberContract))]
    public class UserDetailContract
    {
        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
    }

    [DataContract]
    public class PasswordUserDetailContract : UserDetailContract
    {
        [DataMember]
        public string PasswordHash { get; set; }

        [DataMember]
        public string PasswordSalt { get; set; }
    }


    [DataContract]
    public class GroupMemberContract : UserDetailContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string AvatarUrl { get; set; }
    }
}