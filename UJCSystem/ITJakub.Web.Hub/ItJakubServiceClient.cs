using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts.Categories;
using ITJakub.Shared.Contracts.Searching;
using log4net;

namespace ITJakub.Web.Hub
{
    public class ItJakubServiceClient:ClientBase<IItJakubService>, IItJakubService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
    }
}