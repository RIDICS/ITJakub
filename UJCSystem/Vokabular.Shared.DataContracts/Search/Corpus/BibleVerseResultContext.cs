using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results", Name = "BibleVerseResultContext")]
    public class BibleVerseResultContext
    {
        [DataMember]
        public string BibleBook { get; set; } 

        [DataMember]
        public string BibleChapter { get; set; }

        [DataMember]
        public string BibleVerse { get; set; }
    }
}