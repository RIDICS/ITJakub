using System.Runtime.Serialization;

namespace ITJakub.Contracts.Searching
{
    [DataContract]
    public class KwicResult
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public KwicStructure Kwic { get; set; }

        [DataMember]
        public string OriginalXml { get; set; }
    }

    [DataContract]
    public class KwicStructure
    {
        [DataMember]
        public string Before { get; set; }

        [DataMember]
        public string Match { get; set; }

        [DataMember]
        public string After { get; set; }
    }
}