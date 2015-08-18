using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public enum FeedbackCategoryEnumContract : byte
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Dictionaries = 1,
        [EnumMember]
        Editions = 2,
        [EnumMember]
        BohemianTextBank = 3,
        [EnumMember]
        OldGrammar = 4,
        [EnumMember]
        ProfessionalLiterature = 5,
        [EnumMember]
        Bibliographies = 6,
        [EnumMember]
        CardFiles = 7,
        [EnumMember]
        AudioBooks = 8,
        [EnumMember]
        Tools = 9,
    }
}