using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class SelectedCategoryCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public IList<int> SelectedCategoryIds { get; set; }

        [DataMember]
        public IList<long> SelectedBookIds { get; set; }
    }
}