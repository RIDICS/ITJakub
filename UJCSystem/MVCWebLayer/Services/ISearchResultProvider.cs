using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.MVCWebLayer.Enums;
using ITJakub.MVCWebLayer.Services.DTOs;

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
        IEnumerable<Source> GetSearchResult();
        Source GetDetail(string id);
        IEnumerable<Source> GetSources(string query, SourcesViewMode mode);
    }
    
}