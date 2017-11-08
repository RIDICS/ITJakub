using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
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