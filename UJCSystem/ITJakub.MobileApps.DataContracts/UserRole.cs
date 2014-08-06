using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public enum UserRole
    {
        [EnumMember]
        Student,
        [EnumMember]
        Teacher
    }
}