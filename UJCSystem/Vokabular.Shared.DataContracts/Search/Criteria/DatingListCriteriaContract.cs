using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "DatingListCriteriaContract")]
    public class DatingListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<DatingCriteriaContract> Disjunctions { get; set; }
    }
}