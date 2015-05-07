using System.Runtime.Serialization;
using Jewelery;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public enum ImageSizeEnum
    {
        [EnumMember] [StringValue("full")] Full,

        [EnumMember] [StringValue("preview")] Preview,

        [EnumMember] [StringValue("thumbnail")] Thumbnail
    }
}