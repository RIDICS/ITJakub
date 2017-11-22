using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types.Favorite
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QueryTypeEnumContract
    {
        Search = 0,
        List = 1,
        Reader = 2,
    }
}