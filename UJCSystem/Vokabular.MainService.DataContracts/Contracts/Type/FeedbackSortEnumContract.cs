using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackSortEnumContract : short
    {
        Date = 0,
        Category = 1,
    }
}
