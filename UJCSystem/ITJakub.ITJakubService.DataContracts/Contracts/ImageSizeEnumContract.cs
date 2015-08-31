using System.Runtime.Serialization;
using Jewelery;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public enum ImageSizeEnum
    {
        [EnumMember] [StringValue("full")] Full,

        [EnumMember] [StringValue("preview")] Preview,

        [EnumMember] [StringValue("thumbnail")] Thumbnail
    }
}