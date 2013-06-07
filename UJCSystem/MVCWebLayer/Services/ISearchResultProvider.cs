using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.Services
{
    public interface ISearchResultProvider
    {
        string[] GetSearchResults(string searchTerm, List<string> categorieIds, List<string> booksIds);
        SearchResult[] GetSearchResultsByType(string book, string searchTerm);
        SearchResult[] GetKwicForKeyWord(string searchTerm);
        SelectionBase[] GetCategoryChildrenById(string categoryId);
        SelectionBase[] GetRootCategories();
    }
}