using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserGroupTypeContract : short
    {
        Role = 0,
        Single = 1,
    }
}