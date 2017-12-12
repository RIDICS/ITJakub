using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CardFileImageSizeEnumContract
    {
        Full,
        Preview,
        Thumbnail,
    }
}