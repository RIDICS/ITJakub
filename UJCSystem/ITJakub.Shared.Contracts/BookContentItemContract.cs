using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class BookContentItemContract
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string ReferredPageXmlId { get; set; }
    }
}