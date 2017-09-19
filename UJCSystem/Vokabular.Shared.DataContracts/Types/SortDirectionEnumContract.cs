using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDirectionEnumContract : short
    {
        Asc = 0,
        Desc = 1,
    }
}