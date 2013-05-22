using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;

namespace ITJakub.MVCWebLayer.Services.Mocks
{
    public class SearchResultsMockProvider : ISearchResultProvider
    {
        private static readonly string[] m_pesQueryResults = new string[] 
            { "pes", "Pes", "peskovati", "peskovati sě", "pesky", "peský", "pessky", "pesský" };
        private static readonly string[] m_janQueryResults = new string[] { "Jan" };

        public string[] GetSearchResults(string query)
        {
            return new string[0];
        }

        public SearchResult[] GetSearchResultsByType(string book, string searchTerm)
        {
            return new SearchResult[0];
        }

        public SearchResult[] GetKwicForKeyWord(string searchTerm)
        {
            return new SearchResult[0];
        }

        public SelectionBase[] GetCategoryChildrenById(string categoryId)
        {
            return new SelectionBase[0];
        }

        public SelectionBase[] GetRootCategories()
        {
            return new SelectionBase[0];
        }
    }


    
}