using System.Collections.Generic;

namespace Vokabular.MainService.Core.Managers.Fulltext.Data
{
    public class FulltextSearchResultData
    {
        public FulltextSearchResultType SearchResultType { get; set; }
        public List<long> LongList { get; set; }
        public List<string> StringList { get; set; }
    }
}