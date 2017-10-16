using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
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