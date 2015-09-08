using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public enum ResourceLevelEnumContract : short
    {
        [EnumMember] Version = 0,
        [EnumMember] Book = 1,
        [EnumMember] Shared = 2,
        [EnumMember] Bibliography = 3,
    }
}