using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types.Favorite
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FavoriteTypeEnumContract
    {
        Unknown = 0,
        Project = 1,
        Category = 2,
        Page = 3,
        Query = 4,
        Snapshot = 5,
        Headword = 6,
    }
}
