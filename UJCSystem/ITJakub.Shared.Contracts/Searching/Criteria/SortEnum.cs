using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public enum SortEnum : short
    {
        [EnumMember]
        Author = 0,
        [EnumMember]
        Title = 1,
        [EnumMember]
        Dating = 2,
    }
}