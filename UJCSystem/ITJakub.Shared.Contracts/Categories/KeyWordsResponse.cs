using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Categories
{
    [DataContract]
    public class KeyWordsResponse
    {
        [DataMember]
        public List<string> FoundTerms { get; set; }

        [DataMember]
        public List<Book> FoundInBooks { get; set; }

        [DataMember]
        public List<SelectionBase> CategoryTree { get; set; }
    }
}