using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortTypeEnumContract : short
    {
        Author = 0,
        Title = 1,
        Dating = 2,
    }
}
