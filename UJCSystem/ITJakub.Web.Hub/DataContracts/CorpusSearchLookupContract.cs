using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.DataContracts
{
    public class CorpusLookupContractBase
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public int ContextLength { get; set; }
    }

    public class CorpusLookupContractSearchGetPage : CorpusLookupContractBase
    {
        public long SnapshotId { get; set; }
    }

    public class CorpusLookupContractBasicSearch : CorpusLookupContractSearchGetPage
    {
        public string Text { get; set; }
    }

    public class CorpusLookupContractAdvancedSearch : CorpusLookupContractSearchGetPage
    {
        public string Json { get; set; }
    }

    public class CorpusListLookupContractBase
    {
        public SortTypeEnumContract SortBooksBy { get; set; }
        public SortDirectionEnumContract SortDirection { get; set; }
        public IList<long> SelectedBookIds { get; set; }
        public IList<int> SelectedCategoryIds { get; set; }
    }

    public class CorpusListLookupBasicSearchParams : CorpusListLookupContractBase
    {
        public string Text { get; set; }
    }

    public class CorpusListLookupAdvancedSearchParams : CorpusListLookupContractBase
    {
        public string Json { get; set; }
    }

    public class CorpusListGetPageContractBasic : CorpusListLookupBasicSearchParams
    {
        public int Start { get; set; }
        public int Count { get; set; }
    }

    public class CorpusListGetPageContractAdvanced : CorpusListLookupAdvancedSearchParams
    {
        public int Start { get; set; }
        public int Count { get; set; }
    }

    public class CorpusSearchTotalResultCountBase
    {
        public IList<long> SelectedSnapshotIds { get; set; }
        public IList<int> SelectedCategoryIds { get; set; }
    }

    public class CorpusSearchTotalResultCountBasic : CorpusSearchTotalResultCountBase
    {
        public string Text { get; set; }
    }


    public class CorpusSearchTotalResultCountAdvanced : CorpusSearchTotalResultCountBase
    {
        public string Json { get; set; }
    }
}