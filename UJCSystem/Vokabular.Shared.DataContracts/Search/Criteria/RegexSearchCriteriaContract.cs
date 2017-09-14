using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract]
    public class RegexSearchCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<string> Disjunctions { get; set; } 
    }
}