using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public enum AuthenticationProviders : byte
    {
        [EnumMember] ItJakub = 0,
        [EnumMember] Gmail = 1,
        [EnumMember] Facebook = 2,
        [EnumMember] LiveId = 3
    }
}