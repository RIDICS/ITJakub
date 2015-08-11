using System.Collections.Generic;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceAuthenticatedManager : IItJakubServiceEncrypted
    {
        private readonly WindsorContainer m_container = Container.Current;
        private readonly UserManager m_userManager;
        private readonly FavoriteManager m_favoriteManager;
        private readonly FeedbackManager m_feedbackManager;

        public ItJakubServiceAuthenticatedManager()
        {
            m_userManager = m_container.Resolve<UserManager>();
            m_favoriteManager = m_container.Resolve<FavoriteManager>();
            m_feedbackManager = m_container.Resolve<FeedbackManager>();
        }

        public UserContract FindUserById(int userId)
        {
            return m_userManager.FindById(userId);
        }

        public UserContract FindUserByUserName(string userName)
        {
            return m_userManager.FindByUserName(userName);
        }

        public UserContract CreateUser(UserContract user)
        {
            return m_userManager.CreateLocalUser(user);
        }

        #region Favorite Items

        public List<PageBookmarkContract> GetPageBookmarks(string bookId, string userName)
        {
            return m_favoriteManager.GetPageBookmarks(bookId, userName);
        }

        public void AddPageBookmark(string bookId, string pageName, string userName)
        {
           m_favoriteManager.AddPageBookmark(bookId,pageName, userName);
        }

        public void RemovePageBookmark(string bookId, string pageName, string userName)
        {
            m_favoriteManager.RemovePageBookmark(bookId, pageName, userName);
        }

        public IList<HeadwordBookmarkContract> GetHeadwordBookmarks(string userName)
        {
            return m_favoriteManager.GetHeadwordBookmarks(userName);
        }

        public void AddHeadwordBookmark(string bookXmlId, string entryXmlId, string userName)
        {
            m_favoriteManager.AddHeadwordBookmark(bookXmlId, entryXmlId, userName);
        }

        public void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId, string userName)
        {
            m_favoriteManager.RemoveHeadwordBookmark(bookXmlId, entryXmlId, userName);
        }

        #endregion

        #region Feedback

        public void CreateFeedback(string feedback, string username)
        {
            m_feedbackManager.CreateFeedback(feedback, username);
        }

        public void CreateFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId,
            string username)
        {
            m_feedbackManager.CreateFeedbackForHeadword(feedback, bookXmlId, versionXmlId, entryXmlId, username);
        }

        #endregion
    }
}