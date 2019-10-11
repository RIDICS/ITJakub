using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProjectTypeContract
    {
        Research = 0,
        Community = 1,
        Bibliography = 2,
    }
}