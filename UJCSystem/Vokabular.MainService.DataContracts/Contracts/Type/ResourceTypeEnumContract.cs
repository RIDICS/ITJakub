using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vokabular.MainService.DataContracts.Contracts.Type
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResourceTypeEnumContract
    {
        None = 0,
        ResourceGroup = 1,
        ProjectMetadata = 2,
        Chapter = 3,
        Page = 4,
        Headword = 5,
        Text = 6,
        Image = 7,
        Audio = 8,
        AudioTrack = 9,
        BookVersion = 10,
        EditionNote = 11
    }
}