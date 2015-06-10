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

        public ItJakubServiceAuthenticatedManager()
        {
            m_userManager = m_container.Resolve<UserManager>();
            m_favoriteManager = m_container.Resolve<FavoriteManager>();
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

        #endregion
    }
}