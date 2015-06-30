using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class HeadwordContract
    {
        [DataMember]
        public string XmlEntryId { get; set; }
        
        [DataMember]
        public string Headword { get; set; }

        [DataMember]
        public HeadwordBookInfoContract BookInfo { get; set; }
    }

    [DataContract]
    public class HeadwordBookInfoContract
    {
        [DataMember]
        public string Guid { get; set; }

        [DataMember]
        public string Acronym { get; set; }
    }
}