using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching
{
    [DataContract]
    public class CriteriumTextElement
    {
        [DataMember]
        public string Value { get; private set; }

        [DataMember]
        public string Lemma { get; set; }

        [DataMember]
        public string Stemma { get; set; }

        public CriteriumTextElement(string value)
        {
            Value = value;
        }
    }
}