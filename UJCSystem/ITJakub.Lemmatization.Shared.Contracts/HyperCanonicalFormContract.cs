using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    public class HyperCanonicalFormContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public HyperCanonicalFormTypeContract Type { get; set; }
    }

    [DataContract]
    public enum HyperCanonicalFormTypeContract : short
    {
        [EnumMember]
        HyperLemma = 0,

        [EnumMember]
        HyperStemma = 1,
    }
}