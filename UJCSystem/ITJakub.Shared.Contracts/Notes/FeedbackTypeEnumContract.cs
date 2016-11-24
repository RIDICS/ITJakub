using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    public enum FeedbackTypeEnumContract
    {
        [EnumMember]
        Generic = 0,

        [EnumMember]
        Headword = 1
    }
}