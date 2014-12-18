using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class SearchResultContract
    {
        [DataMember]
        public string BookId { get; set; }

        [DataMember]
        public string BookType { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}