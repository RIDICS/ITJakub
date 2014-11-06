using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching
{
    [DataContract]
    public class SearchRequest
    {
        [DataMember]
        public IEnumerable<SearchCriteriumBase> Criteria { get; private set; }

        public SearchRequest()
        {
            Criteria = new List<SearchCriteriumBase>();
        }
    }
}