using System.Collections.Generic;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class CorpusSearchResultDataList
    {
        public FulltextSearchResultType SearchResultType { get; set; }
        
        public List<CorpusSearchResultData> List { get; set; }
    }
}