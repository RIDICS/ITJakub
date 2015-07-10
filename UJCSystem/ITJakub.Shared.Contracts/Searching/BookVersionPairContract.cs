using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching
{
    [DataContract]
    public class BookVersionPairContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string VersionId { get; set; }
    }
}