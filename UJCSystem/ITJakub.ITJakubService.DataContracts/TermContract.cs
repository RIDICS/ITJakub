using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class TermContract
    {
        [DataMember]
        public string XmlId { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public long Position { get; set; }
    }
}