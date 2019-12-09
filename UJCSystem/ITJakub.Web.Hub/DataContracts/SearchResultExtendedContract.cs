using Vokabular.MainService.DataContracts.Contracts.Search;

namespace ITJakub.Web.Hub.DataContracts
{
    public class SearchResultExtendedContract : SearchResultContract
    {
        public string ProjectTypeString { get; set; }

        public string TextTypeString { get; set; }

        public string CreateTimeString { get; set; }
    }

    public class SearchResultDetailExtendedContract : SearchResultDetailContract
    {
        public string ProjectTypeString { get; set; }

        public string TextTypeString { get; set; }

        public string CreateTimeString { get; set; }
    }

    public class AudioBookSearchResultExtendedContract : AudioBookSearchResultContract
    {
        public string ProjectTypeString { get; set; }

        public string TextTypeString { get; set; }

        public string CreateTimeString { get; set; }
    }
}