using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
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