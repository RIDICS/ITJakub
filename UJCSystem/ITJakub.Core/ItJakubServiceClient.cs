using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.Contracts;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using log4net;

namespace ITJakub.Core
{
    public class ItJakubServiceClient:ClientBase<IItJakubService>, IItJakubService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<string> GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds)
        {
            try
            {
                return Channel.GetAllExtendedTermsForKey(key, categorieIds, booksIds);
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

        public SelectionBase[] GetCategoryChildrenById(string categoryId)
        {
            try
            {
                return Channel.GetCategoryChildrenById(categoryId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCategoryChildrenById failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCategoryChildrenById timeouted with: {0}", ex);
                throw;
            }
        }

        public SelectionBase[] GetRootCategories()
        {
            try
            {
                return Channel.GetRootCategories();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetRootCategories failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetRootCategories timeouted with: {0}", ex);
                throw;
            }

        }

    }
}