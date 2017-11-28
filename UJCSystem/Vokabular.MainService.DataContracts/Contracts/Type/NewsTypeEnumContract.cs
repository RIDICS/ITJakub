using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NewsTypeEnumContract
    {
        Combined = 0,
        Web = 1,
        MobileApps = 2,
    }
}
