using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Vokabular.Shared.DataContracts.Attribute;

namespace Vokabular.MainService.DataContracts.Contracts.CardFile
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CardImageSizeEnumContract
    {
        [EnumMember] [StringValue("full")] Full,

        [EnumMember] [StringValue("preview")] Preview,

        [EnumMember] [StringValue("thumbnail")] Thumbnail
    }
}