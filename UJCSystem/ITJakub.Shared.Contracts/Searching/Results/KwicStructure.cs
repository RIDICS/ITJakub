using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
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