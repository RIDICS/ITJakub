using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Categories
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