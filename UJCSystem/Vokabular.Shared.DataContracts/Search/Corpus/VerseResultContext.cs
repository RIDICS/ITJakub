using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results", Name = "VerseResultContext")]
    public class VerseResultContext
    {
        [DataMember]
        public string VerseXmlId { get; set; }

        [DataMember]
        public string VerseName { get; set; }
    }
}