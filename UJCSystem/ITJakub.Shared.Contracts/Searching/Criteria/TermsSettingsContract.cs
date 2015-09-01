using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class TermsSettingsContract
    {
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public int Start { get; set; }
    }
}