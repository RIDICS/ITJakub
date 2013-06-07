﻿using System.Collections.Generic;
using System.ServiceModel;
using ITJakub.Contracts;
using ITJakub.Contracts.Searching;

namespace ITJakub.SearchService
{
    public class SearchService : ISearchServiceLocal
    {
        private readonly SearchServiceManager m_searchServiceManager;

        public SearchService()
        {
            m_searchServiceManager = Container.Current.Resolve<SearchServiceManager>();
        }

        public void Search(List<SearchCriteriumBase> criteria)
        {
            m_searchServiceManager.Search(criteria);
        }

        public SearchResult[] GetContextForKeyWord(string keyWord)
        {
            return m_searchServiceManager.GetContextForKeyWord(keyWord);
        }

        public string GetTitleById(string id)
        {
            return m_searchServiceManager.GetTitleById(id);
        }

        public List<string> AllExtendedTermsForKey(string key)
        {
            return m_searchServiceManager.AllExtendedTermsForKey(key);
        }

        public List<string> AllExtendedTermsForKeyWithBooksRestriction(string key, List<string> booksIds)
        {
            return m_searchServiceManager.AllExtendedTermsForKeyWithBooksRestriction(key, booksIds);
        }

    }

    [ServiceContract]
    public interface ISearchServiceLocal:ISearchService
    {
    }
}
