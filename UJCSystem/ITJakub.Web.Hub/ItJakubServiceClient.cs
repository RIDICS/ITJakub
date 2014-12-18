using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.Web.Hub
{
    public class ItJakubServiceClient : ClientBase<IItJakubService>, IItJakubService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CreateUserResultContract CreateUser(CreateUserContract createUserContract)
        {
            try
            {
                return Channel.CreateUser(createUserContract);
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

        public LoginUserResultContract LoginUser(LoginUserContract loginUserContract)
        {
            try
            {
                return Channel.LoginUser(loginUserContract);
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

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            try
            {
                return Channel.GetAllAuthors();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetAllAuthors failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetAllAuthors timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPageByName(string documentId, string pageName, string resultFormat)
        {
            try
            {
                return Channel.GetBookPageByName(documentId, pageName, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByName failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByName timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, string resultFormat)
        {
            try
            {
                return Channel.GetBookPagesByName(documentId, startPageName, endPageName, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPagesByName failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPagesByName timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPageByPosition(string documentId, int position, string resultFormat)
        {
            try
            {
                return Channel.GetBookPageByPosition(documentId, position, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByPosition failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByPosition timeouted with: {0}", ex);
                throw;
            }
        }

        public IList<BookPage> GetBookPageList(string documentId)
        {
            try
            {
                return Channel.GetBookPageList(documentId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageList failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageList timeouted with: {0}", ex);
                throw;
            }
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            try
            {
                Channel.AddResource(resourceInfoSkeleton);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource timeouted with: {0}", ex);
                throw;
            }
        }

        public bool ProcessSession(string sessionId, string uploadMessage)
        {
            try
            {
                return Channel.ProcessSession(sessionId, uploadMessage);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession timeouted with: {0}", ex);
                throw;
            }
        }

        public List<SearchResultContract> Search(string term)
        {
            try
            {
                return Channel.Search(term);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
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