using System.Collections.Generic;
using ITJakub.Contracts.Searching;

namespace ITJakub.Core
{
    public class ItJakubServiceManager
    {
        private readonly SearchServiceClient m_searchClient;

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
    }
}