using System.Collections.Generic;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Database;

namespace ITJakub.Core
{
    public class ItJakubServiceManager
    {
        private readonly SearchServiceClient m_searchClient;
        private readonly ReleationDatabaseMock m_releationDatabaseMock;

        public ItJakubServiceManager(SearchServiceClient searchClient)
        {
            m_searchClient = searchClient;
        }

        public List<string> GetAllExtendedTermsForKey(string key)
        {
            return m_searchClient.AllExtendedTermsForKey(key);
        }

        public SearchResult[] GetContextForKeyWord(string keyWord)
        {
            return m_searchClient.GetContextForKeyWord(keyWord);
        }

        public SelectionBase[] GetCategoryChildrenById(string categoryId)
        {
            return m_releationDatabaseMock.GetChildren(categoryId).ToArray();
        }
    }
}