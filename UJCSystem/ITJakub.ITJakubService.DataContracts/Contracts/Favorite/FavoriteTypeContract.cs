using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts.Favorite
{
    [DataContract]
    public enum FavoriteTypeContract
    {
        [EnumMember]
        Unknown = 0,

        [EnumMember]
        Book = 1,

        [EnumMember]
        Category = 2,

        [EnumMember]
        PageBookmark = 3,

        [EnumMember]
        Query = 4,

        [EnumMember]
        BookVersion = 5,

        [EnumMember]
        HeadwordBookmark = 6,
    }
}
