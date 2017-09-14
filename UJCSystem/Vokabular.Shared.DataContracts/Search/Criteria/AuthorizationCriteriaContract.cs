using System.Runtime.Serialization;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.Criteria
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