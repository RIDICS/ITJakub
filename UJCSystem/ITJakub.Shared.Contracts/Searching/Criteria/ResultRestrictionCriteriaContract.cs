using System.Collections.Generic;
using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class ResultRestrictionCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.ResultRestriction; }
        }

        [DataMember]
        public IList<BookVersionPairContract> ResultBooks { get; set; }
    }
}