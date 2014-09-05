using System.Runtime.Serialization;

namespace ITJakub.MobileApps.DataContracts
{
    [DataContract]
    public enum AuthProvidersContract : byte
    {
        [EnumMember]
        ItJakub = 0,


        [EnumMember]
        Facebook = 1,


        [EnumMember]
        Google = 2,
        
        [EnumMember]
        LiveId = 3
    }
}