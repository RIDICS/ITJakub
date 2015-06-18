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
        IEnumerable<SearchResult> GetSearchResult(string searchTerm);
        SearchResult GetDetail(string id);
        IEnumerable<SearchResult> GetSourcesTitleByLetter(string letter);
        IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter);
        string GetContentByBookId(string id);

        List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string searchTerm, string booksIds);
    }
    
}