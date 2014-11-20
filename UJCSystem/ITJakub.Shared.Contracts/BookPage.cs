using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class BookPage
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public int Position { get; set; }

    }
}