using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    public class CanonicalFormContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public CanonicalFormTypeContract Type { get; set; }
    }

    [DataContract]
    public enum CanonicalFormTypeContract : short
    {
        [EnumMember]
        Lemma = 0,

        [EnumMember]
        Stemma = 1,

        [EnumMember]
        LemmaOld = 2,

        [EnumMember]
        StemmaOld = 3,
    }
}