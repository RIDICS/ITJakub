using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "ResultRestrictionCriteriaContract")]
    public class ResultRestrictionCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key => CriteriaKey.ResultRestriction;

        [DataMember]
        public IList<BookVersionPairContract> ResultBooks { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "SnapshotResultRestrictionCriteriaContract")]
    public class SnapshotResultRestrictionCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key => CriteriaKey.SnapshotResultRestriction;

        [DataMember]
        public IList<long> SnapshotIds { get; set; }
    }
}