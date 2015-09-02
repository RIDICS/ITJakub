using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public enum AuthProviderEnumContract
    {
        [EnumMember] ItJakub = 0,

        [EnumMember] Google = 1,

        [EnumMember] Facebook = 2,
    }
}