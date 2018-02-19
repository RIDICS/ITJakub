using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Corpus;

namespace Vokabular.Shared.DataContracts.Search
{
    public class HitsWithPageContextResultContract
    {
        public List<PageResultContextData> ResultList { get; set; }
    }

    public class PageResultContextData
    {
        public string PageExternalId { get; set; }

        public KwicStructure ContextStructure { get; set; }
    }
}