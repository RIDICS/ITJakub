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

        public KeyWordsResponse GetAllExtendedTermsForKey(string key, List<string> categorieIds, List<string> booksIds)
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



        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord, List<string> categorieIds, List<string> booksIds)
        {
            try
            {
                return Channel.GetHtmlContextForKeyWord(keyWord, categorieIds, booksIds);
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

        public List<SearchResultWithHtmlContext> GetResultsByBooks(string book, string keyWord)
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

        public List<SelectionBase> GetCategoryChildrenById(string categoryId)
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

        public List<SelectionBase> GetRootCategories()
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

        public IEnumerable<SearchResult> GetBooksBySearchTerm(string searchTerm)
        {
            try
            {
                return Channel.GetBooksBySearchTerm(searchTerm);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksBySearchTerm failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksBySearchTerm timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<SearchResult> GetBooksTitleByLetter(string letter)
        {
            try
            {
                return Channel.GetBooksTitleByLetter(letter);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksTitleByLetter failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksBySearchTerm timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<SearchResult> GetSourcesAuthorByLetter(string letter)
        {
            try
            {
                return Channel.GetSourcesAuthorByLetter(letter);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetSourcesAuthorByLetter failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetSourcesAuthorByLetter timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetContentByBookId(string id)
        {
            try
            {
                return Channel.GetContentByBookId(id);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetContentByBookId failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetContentByBookId timeouted with: {0}", ex);
                throw;
            }
        }

        public SearchResult GetBookById(string id)
        {
            try
            {
                return Channel.GetBookById(id);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookById failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookById timeouted with: {0}", ex);
                throw;
            }
        }
    }
}