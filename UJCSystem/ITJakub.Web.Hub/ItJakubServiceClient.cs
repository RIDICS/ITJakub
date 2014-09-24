using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using ITJakub.Contracts.Categories;
using ITJakub.Contracts.Searching;
using ITJakub.ITJakubService.DataContracts;
using log4net;

namespace ITJakub.Web.Hub
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

        public void CreateUser(AuthProvidersContract providerContract, string providerToken, UserDetailContract userDetail)
        {
            try
            {
                Channel.CreateUser(providerContract, providerToken, userDetail);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("CreateUser failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("CreateUser timeouted with: {0}", ex);
                throw;
            }
        }

        public void LoginUser(AuthProvidersContract providerContract, string providerToken, string email)
        {
            try
            {
               Channel.LoginUser(providerContract, providerToken, email);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("LoginUser failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("LoginUser timeouted with: {0}", ex);
                throw;
            }
        }

        public ProcessedFileInfoContract ProcessUploadedFile(UploadFileContract dataStream)
        {
            try
            {
                return Channel.ProcessUploadedFile(dataStream);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessUploadedFile failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessUploadedFile timeouted with: {0}", ex);
                throw;
            }
        }

        public void SaveFrontImageForFile(UploadImageContract uploadImageContract)
        {
            try
            {
                Channel.SaveFrontImageForFile(uploadImageContract);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SaveFrontImageForFile failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SaveFrontImageForFile timeouted with: {0}", ex);
                throw;
            }
        }

        public void SavePageImageForFile(UploadImageContract uploadImageContract)
        {
            try
            {
                Channel.SavePageImageForFile(uploadImageContract);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SavePageImageForFile failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SavePageImageForFile timeouted with: {0}", ex);
                throw;
            }
        }

        public void SaveFileMetadata(string fileGuid, string name, string author)
        {
            try
            {
                Channel.SaveFileMetadata(fileGuid, name, author);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SaveFileMetadata failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SaveFileMetadata timeouted with: {0}", ex);
                throw;
            }
        }
    }
}