using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedbackTypeEnumContract
    {
        Generic = 0,
        Headword = 1
    }
}