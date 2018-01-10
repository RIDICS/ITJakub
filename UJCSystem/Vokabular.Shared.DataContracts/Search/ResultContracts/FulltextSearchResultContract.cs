using System.Collections.Generic;

namespace Vokabular.Shared.DataContracts.Search
{
    public class FulltextSearchResultContract
    {
        public long Count { get; set; }
        public List<long> ProjectIds { get; set; }
    }
}