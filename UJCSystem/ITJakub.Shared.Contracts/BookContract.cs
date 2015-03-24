using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class BookContract
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int CategoryId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }

    }
}