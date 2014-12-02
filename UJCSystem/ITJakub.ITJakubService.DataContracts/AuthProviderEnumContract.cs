using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public enum AuthProviderEnumContract
    {
        [EnumMember] ItJakub = 0,

        [EnumMember] Google = 1,

        [EnumMember] Facebook = 2,
    }
}