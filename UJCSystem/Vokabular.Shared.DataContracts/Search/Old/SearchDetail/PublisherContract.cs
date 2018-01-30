using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old.SearchDetail
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts", Name = "PublisherContract")]
    public class PublisherContract
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Email { get; set; }
    }
}