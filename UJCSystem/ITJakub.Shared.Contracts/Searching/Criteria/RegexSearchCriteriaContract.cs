using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class RegexSearchCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<string> Disjunctions { get; set; } 
    }
}