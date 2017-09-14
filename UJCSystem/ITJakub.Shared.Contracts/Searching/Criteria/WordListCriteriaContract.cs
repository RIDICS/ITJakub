using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class WordListCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<WordCriteriaContract> Disjunctions { get; set; }
    }
}