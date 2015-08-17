using System.Runtime.Serialization;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [DataContract]
    public class LemmatizationTypeaheadContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}