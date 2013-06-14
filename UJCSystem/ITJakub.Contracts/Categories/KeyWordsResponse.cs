using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Categories
{
    [DataContract]
    public class KeyWordsResponse
    {
        [DataMember]
        public string[] FoundTerms { get; set; }

        [DataMember]
        public List<SelectionBase> CategoryTree { get; set; }
    }
}