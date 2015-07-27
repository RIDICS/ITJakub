using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class HeadwordBookmarkContract
    {
        [DataMember]
        public string EntryXmlId { get; set; }

        [DataMember]
        public string BookId { get; set; }
    }
}