using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types.Favorite
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