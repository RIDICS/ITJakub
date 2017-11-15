using System.Collections.Generic;
using Vokabular.MainService.Core.Managers.Fulltext.Data;

namespace Vokabular.Shared.DataContracts.Search.ResultContracts
{
    public class CorpusSearchResultDataList
    {
        public FulltextSearchResultType SearchResultType { get; set; }
        
        public List<CorpusSearchResultData> List { get; set; }
    }
}