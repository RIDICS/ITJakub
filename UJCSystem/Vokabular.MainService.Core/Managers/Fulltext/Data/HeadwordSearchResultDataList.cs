using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.ResultContracts;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class HeadwordSearchResultDataList
    {
        public FulltextSearchResultType SearchResultType { get; set; }

        public List<HeadwordDictionaryEntryData> List { get; set; }
    }
}