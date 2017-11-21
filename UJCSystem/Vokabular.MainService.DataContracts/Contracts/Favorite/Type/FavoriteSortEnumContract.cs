using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FavoriteSortEnumContract
    {
        TitleAsc = 0,
        TitleDesc = 1,
        CreateTimeAsc = 2,
        CreateTimeDesc = 3
    }
}