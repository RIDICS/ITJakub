using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Criteria", Name = "RegexTokenDistanceCriteriaContract")]
    public class RegexTokenDistanceCriteriaContract
    {
        [DataMember]
        public int Distance { get; set; }

        [DataMember]
        public string FirstRegex { get; set; }

        [DataMember]
        public string SecondRegex { get; set; }
    }
}