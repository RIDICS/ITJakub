using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.Criteria
{
    [DataContract]
    public class ResultRestrictionCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key => CriteriaKey.ResultRestriction;

        [DataMember]
        public IList<BookVersionPairContract> ResultBooks { get; set; }
    }

    [DataContract]
    public class NewResultRestrictionCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key => CriteriaKey.NewResultRestriction;

        [DataMember]
        public IList<long> ProjectIds { get; set; }
    }
}