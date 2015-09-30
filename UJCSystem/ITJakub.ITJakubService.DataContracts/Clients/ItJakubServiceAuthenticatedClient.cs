using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts.News;
using ITJakub.Shared.Contracts.Notes;
using log4net;

namespace ITJakub.ITJakubService.DataContracts.Clients
{
    public class ItJakubServiceAuthenticatedClient : ClientBase<IItJakubServiceAuthenticated>, IItJakubServiceAuthenticated
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<PageBookmarkContract> GetPageBookmarks(string bookId, string userName)
        {
            try
            {
                return Channel.GetPageBookmarks(bookId, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void AddPageBookmark(string bookId, string pageName, string userName)
        {
            try
            {
                Channel.AddPageBookmark(bookId, pageName, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void RemovePageBookmark(string bookId, string pageName, string userName)
        {
            try
            {
                Channel.RemovePageBookmark(bookId, pageName, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public IList<HeadwordBookmarkContract> GetHeadwordBookmarks(string userName)
        {
            try
            {
                return Channel.GetHeadwordBookmarks(userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void AddHeadwordBookmark(string bookXmlId, string entryXmlId, string userName)
        {
            try
            {
                Channel.AddHeadwordBookmark(bookXmlId, entryXmlId, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId, string userName)
        {
            try
            {
                Channel.RemoveHeadwordBookmark(bookXmlId, entryXmlId, userName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void CreateFeedback(string feedback, string username, FeedbackCategoryEnumContract category)
        {
            try
            {
                Channel.CreateFeedback(feedback, username, category);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void CreateFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string username)
        {
            try
            {
                Channel.CreateFeedbackForHeadword(feedback, bookXmlId, versionXmlId, entryXmlId, username);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void CreateNewsSyndicationItem(string title, string content, string url, NewsTypeContract itemType, string username)
        {
            try
            {
                Channel.CreateNewsSyndicationItem(title, content, url, itemType, username);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} timeouted with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        private string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }
    }
}