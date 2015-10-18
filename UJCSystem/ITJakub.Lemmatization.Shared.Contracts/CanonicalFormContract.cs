using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(CanonicalFormContract))]
    [KnownType(typeof(InverseCanonicalFormContract))]
    public class CanonicalFormTypeaheadContract
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
    public class CanonicalFormContract : CanonicalFormTypeaheadContract
    {
        [DataMember]
        public HyperCanonicalFormContract HyperCanonicalForm { get; set; }
    }

    [DataContract]
    public class InverseCanonicalFormContract : CanonicalFormTypeaheadContract
    {
        [DataMember]
        public TokenCharacteristicContract CanonicalFormFor { get; set; }
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