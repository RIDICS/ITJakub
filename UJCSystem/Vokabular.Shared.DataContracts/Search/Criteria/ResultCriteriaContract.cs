using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Search.OldCriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "ResultCriteriaContract")]
    public class ResultCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.Result; }
        }

        [DataMember]
        public int? Start { get; set; }

        [DataMember]
        public int? Count { get; set; }

        [DataMember]
        public HitSettingsContract HitSettingsContract { get; set; }

        [DataMember]
        public TermsSettingsContract TermsSettingsContract { get; set; }

        [DataMember]
        public SortEnum? Sorting { get; set; }

        [DataMember]
        public ListSortDirection Direction { get; set; }
    }
}