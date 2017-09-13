using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class SelectedCategoryCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.SelectedCategory; }
        }

        [DataMember]
        public IList<int> SelectedCategoryIds { get; set; }

        [DataMember]
        public IList<long> SelectedBookIds { get; set; }
    }
}