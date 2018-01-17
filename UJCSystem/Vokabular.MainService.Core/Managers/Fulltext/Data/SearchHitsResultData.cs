using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class SearchHitsResultData
    {
        public PageSearchResultType SearchResultType { get; set; }

        public List<PageResultContextData> ResultList { get; set; }
    }

    public class PageResultContextData
    {
        public long LongId { get; set; }

        public string StringId { get; set; }

        public KwicStructure ContextStructure { get; set; }
    }
}