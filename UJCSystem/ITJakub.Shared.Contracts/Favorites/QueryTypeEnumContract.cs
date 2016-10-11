using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Favorites
{
    [DataContract]
    public enum QueryTypeEnumContract : short
    {
        [EnumMember]
        Search = 0,

        [EnumMember]
        List = 1,

        [EnumMember]
        Reader = 2,
    }
}
