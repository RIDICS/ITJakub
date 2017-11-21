using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Favorite.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QueryTypeEnumContract
    {
        Search = 0,
        List = 1,
        Reader = 2,
    }
}