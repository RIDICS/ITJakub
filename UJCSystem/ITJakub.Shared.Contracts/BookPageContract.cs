using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class BookPageContract
    {
        [DataMember]
        public string XmlId { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public int Position { get; set; }
    }
}