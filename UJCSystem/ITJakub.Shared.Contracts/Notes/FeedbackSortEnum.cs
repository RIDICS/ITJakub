using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public enum FeedbackSortEnum : short
    {
        [EnumMember]
        Author = 0,
        [EnumMember]
        Email = 1,
        [EnumMember]
        Category = 2,
        [EnumMember]
        Dating = 3,
    }
}