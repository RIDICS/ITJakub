using System.Runtime.Serialization;

namespace ITJakub.MobileApps.MobileContracts
{
    [DataContract]
    public enum CategoryContract
    {
        [EnumMember]
        Dictionary,

        [EnumMember]
        Edition,

        [EnumMember]
        OldBohemainTextBank,

        [EnumMember]
        OldGrammar,

        [EnumMember]
        ProfessionalLiterature
    }
}