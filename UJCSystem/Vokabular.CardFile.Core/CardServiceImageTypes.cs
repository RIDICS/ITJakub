using Vokabular.Shared.DataContracts.Attribute;

namespace Vokabular.CardFile.Core
{
    public enum CardServiceImageTypes
    {
        [StringValue("full")]
        Full,

        [StringValue("preview")]
        Preview,

        [StringValue("thumbnail")]
        Thumbnail,
    }
}