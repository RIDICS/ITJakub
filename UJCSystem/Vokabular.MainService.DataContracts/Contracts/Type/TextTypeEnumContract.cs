using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TextTypeEnumContract : short
    {
        Original = 0,
        Transliterated = 1,
        Transcribed = 2
    }
}