using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts.Contracts
{
    [DataContract]
    public class HeadwordBookmarkContract
    {
        [DataMember]
        public string EntryXmlId { get; set; }

        [DataMember]
        public string BookId { get; set; }

        [DataMember]
        public string Headword { get; set; }
    }
}