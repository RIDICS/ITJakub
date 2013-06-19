using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.Services
{
    public interface ISearchResultProvider
    {
        KeyWordsResponse GetSearchResults(string searchTerm, List<string> categorieIds, List<string> booksIds);
        List<SearchResultWithHtmlContext> GetSearchResultsByType(string book, string searchTerm);
        List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string searchTerm);
        SelectionBase[] GetCategoryChildrenById(string categoryId);
        SelectionBase[] GetRootCategories();
    }

    
}