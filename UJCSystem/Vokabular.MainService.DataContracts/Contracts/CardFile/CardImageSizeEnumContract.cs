using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Attribute;

namespace Vokabular.MainService.DataContracts.Contracts.CardFile
{
    [DataContract]
    public enum CardImageSizeEnumContract
    {
        [EnumMember] [StringValue("full")] Full,

        [EnumMember] [StringValue("preview")] Preview,

        [EnumMember] [StringValue("thumbnail")] Thumbnail
    }
}