using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDirectionEnumContract : short
    {
        Asc = 0,
        Desc = 1,
    }
}