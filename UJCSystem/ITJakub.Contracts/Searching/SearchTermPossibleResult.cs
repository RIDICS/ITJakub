using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Searching
{
    [DataContract]
    public class SearchTermPossibleResult
    {
        [DataMember]
        public List<string> AllPossibleTerms { get; set; }

        [DataMember]
        public List<string> AllPossibleBookIds { get; set; }
    }
}