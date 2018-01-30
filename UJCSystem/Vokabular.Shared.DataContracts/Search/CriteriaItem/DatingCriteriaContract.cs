using System;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "DatingCriteriaContract")]
    public class DatingCriteriaContract
    {
        [DataMember]
        public DateTime? NotBefore { get; set; }

        [DataMember]
        public DateTime? NotAfter { get; set; }
    }
}