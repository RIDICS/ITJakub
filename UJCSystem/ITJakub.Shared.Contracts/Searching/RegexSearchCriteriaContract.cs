using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching
{
    [DataContract]
    public class RegexSearchCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public string RegexContent { get; set; }
    }

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

    [DataContract]
    public class RegexTokenListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<RegexTokenDistanceCriteriaContract> Disjunctions { get; set; }
    }
}