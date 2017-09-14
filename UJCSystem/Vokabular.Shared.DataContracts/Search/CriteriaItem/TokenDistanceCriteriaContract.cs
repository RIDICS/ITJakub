using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.CriteriaItem
{
    [DataContract]
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