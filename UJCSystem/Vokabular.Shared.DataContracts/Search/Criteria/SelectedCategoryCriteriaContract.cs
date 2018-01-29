using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "SelectedCategoryCriteriaContract")]
    public class SelectedCategoryCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.SelectedCategory; }
        }

        [DataMember]
        public BookTypeEnumContract? BookType { get; set; }

        [DataMember]
        public IList<int> SelectedCategoryIds { get; set; }

        [DataMember]
        public IList<long> SelectedBookIds { get; set; }
    }
}