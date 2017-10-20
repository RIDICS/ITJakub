using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
    [DataContract]
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