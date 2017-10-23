using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.Shared.DataContracts.Types
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TextFormatEnumContract : byte
    {
        Raw = 0,
        Html = 1,
        Rtf = 2,
    }
}
