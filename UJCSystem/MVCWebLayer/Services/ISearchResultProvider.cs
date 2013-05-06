using System.Collections.Generic;
using ITJakub.Contracts.Searching;
using Ujc.Naki.MVCWebLayer.Services.Enums;

namespace Ujc.Naki.MVCWebLayer.Services
{
    public interface ISearchResultProvider
    {
        string[] GetSearchResults(string query);
        Dictionary<BookCategory, List<string>> GetSearchResultsByType(string query);
        SearchResult[] GetKwicForKeyWord(string searchTerm);
    }
}