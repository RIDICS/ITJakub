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
}
