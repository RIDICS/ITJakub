using System.Collections.Generic;
using ITJakub.Contracts.Searching;

namespace Ujc.Naki.MVCWebLayer.Services
{
    public interface ISearchResultProvider
    {
        string[] GetSearchResults(string query);
        SearchResult[] GetSearchResultsByType(string book, string searchTerm);
        SearchResult[] GetKwicForKeyWord(string searchTerm);
    }
}