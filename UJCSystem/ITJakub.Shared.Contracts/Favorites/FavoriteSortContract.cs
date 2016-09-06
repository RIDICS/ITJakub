using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Favorites
{
    [DataContract]
    public enum FavoriteSortContract
    {
        [EnumMember]
        TitleAsc = 0,

        [EnumMember]
        TitleDesc = 1,

        [EnumMember]
        CreateTimeAsc = 2,

        [EnumMember]
        CreateTimeDesc = 3
    }
}
