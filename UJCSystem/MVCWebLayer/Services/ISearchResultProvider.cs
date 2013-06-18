using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.Services
{
    public interface ISearchResultProvider
    {
        KeyWordsResponse GetSearchResults(string searchTerm, List<string> categorieIds, List<string> booksIds);
        SearchResult[] GetSearchResultsByType(string book, string searchTerm);
        SearchResult[] GetKwicForKeyWord(string searchTerm);
        SearchResult[] GetXmlForKeyWord(string searchTerm);
        SelectionBase[] GetCategoryChildrenById(string categoryId);
        SelectionBase[] GetRootCategories();
    }

    
}