using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "RegexSearchCriteriaContract")]
    public class RegexSearchCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<string> Disjunctions { get; set; } 
    }
}