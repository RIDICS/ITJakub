using System.Runtime.Serialization;

namespace ITJakub.SearchService.DataContracts.Types
{
    [DataContract]
    public enum OutputFormatEnumContract : short
    {
        [EnumMember] Xml = 0,
        [EnumMember] Html = 1,
        [EnumMember] Rtf = 2,
		[EnumMember] Pdf = 3
    }
}