using System;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
{
    [DataContract]
    public class DatingCriteriaContract : SearchCriteriaContract
    {
        [DataMember]
        public DateTime? NotBefore { get; set; }

        [DataMember]
        public DateTime? NotAfter { get; set; }
    }
}