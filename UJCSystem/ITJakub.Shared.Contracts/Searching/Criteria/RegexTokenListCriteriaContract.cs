using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class RegexTokenListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<RegexTokenDistanceCriteriaContract> Disjunctions { get; set; }
    }
}