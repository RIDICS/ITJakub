using System.Collections.Generic;
using Newtonsoft.Json;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Utils;

namespace Vokabular.Shared.DataContracts.Search.RequestContracts
{
    public class SearchRequestContractBase
    {
        public int? Start { get; set; }

        public int? Count { get; set; }

        [JsonConverter(typeof(SearchCriteriaJsonConverter))]
        public IList<SearchCriteriaContract> ConditionConjunction { get; set; }
    }

    public class SearchRequestContract : SearchRequestContractBase
    {
        public bool FetchTerms { get; set; }

        public SortTypeEnumContract? Sort { get; set; }

        public SortDirectionEnumContract? SortDirection { get; set; }
    }

    public class HeadwordSearchRequestContract : SearchRequestContractBase
    {
    }

    public class CorpusSearchRequestContract : SearchRequestContractBase
    {
        //public HitSettingsContract HitSettingsContract { get; set; } HitSettings is redundant in direct corpus search

        public int ContextLength { get; set; }
    }
}
