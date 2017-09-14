using System;
using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract]
    public class DatingCriteriaContract
    {
        [DataMember]
        public DateTime? NotBefore { get; set; }

        [DataMember]
        public DateTime? NotAfter { get; set; }
    }
}