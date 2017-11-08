using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old.SearchDetail
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