using System.Collections.Generic;

namespace Vokabular.Shared.DataContracts.Search.ResultContracts
{
    public class PageSearchResultData
    {
        public PageSearchResultType SearchResultType { get; set; }
        public List<long> LongList { get; set; }
        public List<string> StringList { get; set; }
    }
}