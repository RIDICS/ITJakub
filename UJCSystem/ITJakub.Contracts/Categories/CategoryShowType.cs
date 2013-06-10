using System.Runtime.Serialization;

namespace ITJakub.Contracts.Categories
{
    [DataContract]
    public enum CategoryShowType
    {
        [EnumMember]
        SelectionBox,
        [EnumMember]
        Enumeration,
    }
}