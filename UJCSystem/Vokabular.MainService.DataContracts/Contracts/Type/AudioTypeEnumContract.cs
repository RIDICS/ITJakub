using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AudioTypeEnumContract
    {
        Unknown = 0,
        Mp3 = 1,
        Ogg = 2,
        Wav = 3,
    }
}
