using Vokabular.DataEntities.Database.Entities;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public interface IFulltextStorage
    {
        ProjectType ProjectType { get; }
        string GetPageText(TextResource textResource, TextFormatEnumContract format);
        string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format, SearchPageRequestContract searchRequest);
        string GetHeadwordText(HeadwordResource headwordResource, TextFormatEnumContract format);
        string GetHeadwordTextFromSearch(HeadwordResource headwordResource, TextFormatEnumContract format, SearchPageRequestContract searchRequest);
    }
}
