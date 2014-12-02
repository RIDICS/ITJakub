using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using log4net;

namespace ITJakub.XmlProcessingConsole
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

        public int CreateAuthor(string name)
        {
            try
            {
                return Channel.CreateAuthor(name);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("CreateAuthor failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("CreateAuthor timeouted with: {0}", ex);
                throw;
            }
        }

        public void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds)
        {
            try
            {
                Channel.AssignAuthorsToBook(bookGuid, bookVersionGuid, authorIds);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AssignAuthorsToBook failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AssignAuthorsToBook timeouted with: {0}", ex);
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

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName,
            string resultFormat)
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
    }
}