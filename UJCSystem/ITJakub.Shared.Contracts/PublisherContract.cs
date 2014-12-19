using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class PublisherContract
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Email { get; set; }
    }
}