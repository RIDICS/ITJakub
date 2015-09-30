using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public enum UserRoleContract
    {
        [EnumMember]
        Student,

        [EnumMember]
        Teacher
    }
}