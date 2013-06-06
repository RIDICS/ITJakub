using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.Contracts;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Searching;
using log4net;

namespace ITJakub.Core
{
    public class SearchServiceClient : ClientBase<ISearchService>, ISearchService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> AllExtendedTermsForKey(string key, List<string> booksIds)
        {
            try
            {
                return Channel.AllExtendedTermsForKey(key, booksIds);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AllExtendedTermsForKey failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AllExtendedTermsForKey timeouted with: {0}", ex);
                throw;
            }
        }

        public void Search(List<SearchCriteriumBase> criteria)
        {
            try
            {
                Channel.Search(criteria); //TODO return some result
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search timeouted with: {0}", ex);
                throw;
            }
        }

        public SearchResult[] GetContextForKeyWord(string keyWord)
        {
            try
            {
                return Channel.GetContextForKeyWord(keyWord);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetTitleById(string id)
        {
            try
            {
                return Channel.GetTitleById(id);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search timeouted with: {0}", ex);
                throw;
            }
        }

     
    }
}