using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

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
        public int UserId { get; set; }
    }
}