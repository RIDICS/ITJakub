using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class RegexTokenDistanceCriteriaContract
    {
        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public string FirstRegex { get; set; }

        [DataMember]
        public string SecondRegex { get; set; }
    }
}