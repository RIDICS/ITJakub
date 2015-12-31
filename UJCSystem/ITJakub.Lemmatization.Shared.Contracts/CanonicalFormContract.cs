using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(CanonicalFormTypeaheadContract))]
    [KnownType(typeof(InverseCanonicalFormContract))]
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
    public class CanonicalFormTypeaheadContract : CanonicalFormContract
    {
        [DataMember]
        public HyperCanonicalFormContract HyperCanonicalForm { get; set; }
    }

    [DataContract]
    public class InverseCanonicalFormContract : CanonicalFormContract
    {
        [DataMember]
        public IList<InverseTokenCharacteristicContract> CanonicalFormFor { get; set; }
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