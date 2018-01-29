using System.Runtime.Serialization;

namespace ITJakub.SearchService.DataContracts.Types
{
    [DataContract]
    public enum OutputFormatEnumContract : short
    {
        [EnumMember] Html = 1,
        [EnumMember] Rtf = 2,
        [EnumMember] Xml = 3,
        [EnumMember] Pdf = 4,
    }
}