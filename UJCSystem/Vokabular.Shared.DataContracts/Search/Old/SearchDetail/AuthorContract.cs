using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old.SearchDetail

{
    [DataContract]
    public class AuthorContract
    {
        [DataMember]
        public string Name { get; set; }
    }
}