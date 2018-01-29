using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old.SearchDetail
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts", Name = "AuthorContract")]
    public class AuthorContract
    {
        [DataMember]
        public string Name { get; set; }
    }
}