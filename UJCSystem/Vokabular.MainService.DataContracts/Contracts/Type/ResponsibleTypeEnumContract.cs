using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResponsibleTypeEnumContract : short
    {
        Unknown = 0,
        Editor = 1,
        Kolace = 2,
    }
}