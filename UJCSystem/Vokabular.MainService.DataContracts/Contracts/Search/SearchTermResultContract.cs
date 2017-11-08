using System.Collections.Generic;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class SearchTermResultContract
    {
        public int PageHitsCount { get; set; }

        public List<PageContract> PageHits { get; set; }
    }
}