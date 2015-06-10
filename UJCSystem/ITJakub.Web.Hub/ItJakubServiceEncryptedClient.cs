using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using log4net;

namespace ITJakub.Web.Hub
{
    public class ItJakubServiceEncryptedClient : ClientBase<IItJakubServiceEncrypted>, IItJakubServiceEncrypted
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserContract FindUserById(int userId)
        {
            try
            {
                return Channel.FindUserById(userId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserById failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserById timeouted with: {0}", ex);
                throw;
            }
        }

        public UserContract FindUserByUserName(string userName)
        {
            try
            {
                return Channel.FindUserByUserName(userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserByUserName failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("FindUserByUserName timeouted with: {0}", ex);
                throw;
            }
        }

        public UserContract CreateUser(UserContract user)
        {
            try
            {
                return Channel.CreateUser(user);
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


        public List<PageBookmarkContract> GetPageBookmarks(string bookId, string userName)
        {
            try
            {
                return Channel.GetPageBookmarks(bookId, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks timeouted with: {0}", ex);
                throw;
            }
        }

        public void AddBookmark(string bookId, string pageName, string userName)
        {
            try
            {
                Channel.AddBookmark(bookId,pageName, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks timeouted with: {0}", ex);
                throw;
            }
        }

        public void RemoveBookmark(string bookId, string pageName, string userName)
        {
            try
            {
                Channel.RemoveBookmark(bookId,pageName, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetPageBookmarks timeouted with: {0}", ex);
                throw;
            }
        
            
        
        }        
    }
}