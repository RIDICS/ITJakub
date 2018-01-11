using System.Collections.Generic;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class FulltextSearchResultContract
    {
        public long Count { get; set; }
        public List<long> ProjectIds { get; set; }
    }
}