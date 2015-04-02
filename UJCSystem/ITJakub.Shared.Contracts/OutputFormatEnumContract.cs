using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public enum OutputFormatEnumContract : short
    {
        [EnumMember] Xml = 0,
        [EnumMember] Html = 1,
        [EnumMember] Rtf = 2
    }
}