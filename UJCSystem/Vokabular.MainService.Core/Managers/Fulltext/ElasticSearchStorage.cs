using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class ElasticSearchStorage : IFulltextStorage
    {
        public ProjectType ProjectType => ProjectType.Community;

        public string GetPageText(TextResource textResource, TextFormatEnumContract format)
        {
            throw new System.NotImplementedException();
        }

        public string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            throw new System.NotImplementedException();
        }

        public string GetHeadwordText(HeadwordResource headwordResource, TextFormatEnumContract format)
        {
            throw new System.NotImplementedException();
        }

        public string GetHeadwordTextFromSearch(HeadwordResource headwordResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}