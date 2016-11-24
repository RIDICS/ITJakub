using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Notes
{
    [DataContract]
    public class FeedbackHeadwordInfoContract
    {
        [DataMember]
        public long HeadwordId { get; set; }

        [DataMember]
        public string Headword { get; set; }

        [DataMember]
        public string DefaultHeadword { get; set; }

        [DataMember]
        public string DictionaryName { get; set; }
    }
}