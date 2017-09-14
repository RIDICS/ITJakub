using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.OldCriteriaItem
{
    [DataContract]
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