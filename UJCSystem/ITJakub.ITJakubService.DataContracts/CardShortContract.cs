using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class CardShortContract
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public string Headword { get; set; }
    }
}