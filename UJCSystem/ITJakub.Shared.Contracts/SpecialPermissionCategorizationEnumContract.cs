using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public enum SpecialPermissionCategorizationEnumContract : byte
    {
        [EnumMember] Action = 0,
        [EnumMember] Autoimport = 1,
        [EnumMember] Read = 2
    }
}