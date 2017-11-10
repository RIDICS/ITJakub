using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "TokenDistanceCriteriaContract")]
    public class TokenDistanceCriteriaContract
    {
        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public WordCriteriaContract First { get; set; }

        [DataMember]
        public WordCriteriaContract Second { get; set; }
    }
}