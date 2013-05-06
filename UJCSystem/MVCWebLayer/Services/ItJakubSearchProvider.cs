using System;
using System.Collections.Generic;
using ITJakub.Contracts.Searching;
using ITJakub.Core;
using Ujc.Naki.MVCWebLayer.Services.Enums;
using Ujc.Naki.MVCWebLayer.Services.Mocks;

namespace Ujc.Naki.MVCWebLayer.Services
{
    public class ItJakubSearchProvider : ISearchResultProvider
    {

        private readonly ItJakubServiceClient m_serviceClient;

        public ItJakubSearchProvider()
        {
            m_serviceClient = Container.Current.Resolve<ItJakubServiceClient>();
        }


        public string[] GetSearchResults(string query)
        {
            List<string> result = m_serviceClient.GetAllExtendedTermsForKey(query);
            return result.ToArray();
        }

        public Dictionary<BookCategory, List<string>> GetSearchResultsByType(string query)
        {
            //todo remove mockup
            ISearchResultProvider resultsProvider = new SearchResultsMockProvider();
            return resultsProvider.GetSearchResultsByType(query);
        }

        public KwicResult[] GetKwicForKeyWord(string searchTerm)
        {
            var result = m_serviceClient.GetContextForKeyWord(searchTerm);
            return result;
        }
    }

    
}