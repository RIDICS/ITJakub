﻿using System;
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
        private SearchResult[] m_searchResult;

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


            if (m_searchResult == null)
                m_searchResult = m_serviceClient.GetContextForKeyWord(query);

            var result = new Dictionary<BookCategory, List<string>>();

            foreach (var searchResult in m_searchResult)
            {
            }

            return result;
        }

        public SearchResult[] GetKwicForKeyWord(string searchTerm)
        {
            if(m_searchResult == null)
                m_searchResult = m_serviceClient.GetContextForKeyWord(searchTerm);
            return m_searchResult;
        }
    }

    
}