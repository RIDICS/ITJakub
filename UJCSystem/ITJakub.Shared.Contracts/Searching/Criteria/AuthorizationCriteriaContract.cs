using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class AuthorizationCriteriaContract : SearchCriteriaContract
    {
        public override CriteriaKey Key
        {
            get { return CriteriaKey.Authorization; }
        }

        [DataMember]
        public long UserId { get; set; }
    }
}