using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.MVCWebLayer.Enums;

namespace ITJakub.MVCWebLayer.Services
{
    public interface ISearchResultProvider
    {
        KeyWordsResponse GetSearchResults(string searchTerm, List<string> categorieIds, List<string> booksIds);
        List<SearchResultWithHtmlContext> GetSearchResultsByType(string book, string searchTerm);
        List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string searchTerm, List<string> categorieIds, List<string> booksIds);
        List<SelectionBase> GetCategoryChildrenById(string categoryId);
        List<SelectionBase> GetRootCategories();
    }

    public interface ISourcesProvider
    {
        IEnumerable<Book> GetSearchResult();
        Book GetDetail(string id);
        IEnumerable<Book> GetSources(string query, SourcesViewMode mode);
    }
    
}