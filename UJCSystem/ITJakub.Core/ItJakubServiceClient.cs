using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.Contracts;
using ITJakub.Contracts.Searching;
using log4net;

namespace ITJakub.Core
{
    public class ItJakubServiceClient:ClientBase<IItJakubService>, IItJakubService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> GetAllExtendedTermsForKey(string key)
        {
            try
            {
                return Channel.GetAllExtendedTermsForKey(key);
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

        public SearchResult[] GetContextForKeyWord(string searchTerm)
        {
            try
            {
                return Channel.GetContextForKeyWord(searchTerm);
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

        public SearchResult[] GetResultsByBooks(string book, string keyWord)
        {
            try
            {
                return Channel.GetResultsByBooks(book, keyWord);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetResultsByBooks failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetResultsByBooks timeouted with: {0}", ex);
                throw;
            }
        }
    }
}