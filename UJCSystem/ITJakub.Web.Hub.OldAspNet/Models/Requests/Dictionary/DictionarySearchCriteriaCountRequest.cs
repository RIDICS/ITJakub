using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Dictionary
{
    public class DictionarySearchCriteriaCountRequest
    {
        public string Json { get; set; }

        public IList<long> SelectedBookIds { get; set; }

        public IList<int> SelectedCategoryIds { get; set; }
    }
}