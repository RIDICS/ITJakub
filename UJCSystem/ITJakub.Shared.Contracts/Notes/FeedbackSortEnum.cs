using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public enum FeedbackSortEnum : short
    {
        [EnumMember]
        Date = 0,
        [EnumMember]
        Category = 1,
    }
}