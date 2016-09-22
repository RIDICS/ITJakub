using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Requests.Dictionary
{
    public class DictionarySearchCriteriaRequest
    {
        public string Json { get; set; }

        public int Start { get; set; }

        public int Count { get; set; }

        public IList<long> SelectedBookIds { get; set; }

        public IList<int> SelectedCategoryIds { get; set; }
    }
}