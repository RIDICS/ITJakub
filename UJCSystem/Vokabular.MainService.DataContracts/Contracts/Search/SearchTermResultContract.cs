namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class SearchTermResultContract
    {
        public int PageHitsCount { get; set; }

        public object PageHits { get; set; }
    }
}