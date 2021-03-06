﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Utils;

namespace Vokabular.Shared.DataContracts.Search.Request
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

    public class SearchHitsRequestContract : SearchRequestContractBase
    {
        public int ContextLength { get; set; }
    }

    public class HeadwordSearchRequestContract : SearchRequestContractBase
    {
    }

    public class CorpusSearchRequestContract : SearchRequestContractBase
    {
        public int ContextLength { get; set; }

        public SortTypeEnumContract? Sort { get; set; }

        public SortDirectionEnumContract? SortDirection { get; set; }
    }

    public class BookPagedCorpusSearchRequestContract : SearchRequestContractBase
    {
        public SortTypeEnumContract? Sort { get; set; }

        public SortDirectionEnumContract? SortDirection { get; set; }

        public bool FetchNumberOfResults { get; set; }
    }

    public class BookPagedCorpusSearchInSnapshotRequestContract : SearchRequestContractBase
    {
        public int ContextLength { get; set; }
    }
}
