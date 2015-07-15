using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class TokenDistanceCriteriaContract
    {
        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public WordCriteriaContract First { get; set; }

        [DataMember]
        public WordCriteriaContract Second { get; set; }
    }
}