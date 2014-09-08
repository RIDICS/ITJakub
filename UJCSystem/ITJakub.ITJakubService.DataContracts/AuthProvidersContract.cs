using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public enum AuthProvidersContract
    {
        [EnumMember]
        ItJakub = 0,

        [EnumMember]
        Facebook = 1,
    }
}