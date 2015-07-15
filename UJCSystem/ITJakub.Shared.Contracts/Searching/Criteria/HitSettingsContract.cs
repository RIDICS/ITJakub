using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Criteria
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