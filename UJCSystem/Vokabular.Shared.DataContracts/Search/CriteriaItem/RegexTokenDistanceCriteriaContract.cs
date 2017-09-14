using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract]
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