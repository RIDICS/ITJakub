using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
{
    [DataContract]
    public class VerseResultContext
    {
        [DataMember]
        public string VerseXmlId { get; set; }

        [DataMember]
        public string VerseName { get; set; }
    }
}