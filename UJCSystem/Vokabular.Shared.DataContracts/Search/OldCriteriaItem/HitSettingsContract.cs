using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.OldCriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "HitSettingsContract")]
    public class HitSettingsContract
    {
        [DataMember]
        public int? Count { get; set; }

        [DataMember]
        public int? Start { get; set; }

        [DataMember]
        public int ContextLength { get; set; }
    }
}