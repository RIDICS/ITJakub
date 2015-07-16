using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class TokenDistanceListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<TokenDistanceCriteriaContract> Disjunctions { get; set; } 
    }
}