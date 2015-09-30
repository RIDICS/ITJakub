using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts;
using log4net;

namespace ITJakub.ITJakubService.Core
{
    public class FavoriteManager
    {
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly UserRepository m_userRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly BookRepository m_bookRepository;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public FavoriteManager(FavoritesRepository favoritesRepository, UserRepository userRepository, BookRepository bookRepository, BookVersionRepository bookVersionRepository)
        {
            m_favoritesRepository = favoritesRepository;
            m_userRepository = userRepository;
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
        }

        public List<PageBookmarkContract> GetPageBookmarks(string bookXmlId, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return new List<PageBookmarkContract>();

            var allBookmarks = m_favoritesRepository.GetAllPageBookmarksByBookId(bookXmlId, userName);
            return Mapper.Map<List<PageBookmarkContract>>(allBookmarks);
        }

        public void AddPageBookmark(string bookXmlId, string pageXmlId, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = "Username is empty, cannot add bookmark";

                if (m_log.IsWarnEnabled)
                    m_log.Warn(message);
                throw new ArgumentException(message);
            }
            User user = m_userRepository.FindByUserName(userName);

            if (user == null)
            {
                string message = string.Format("Cannot locate user by username: '{0}'", userName);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var bookPage = m_bookVersionRepository.GetPageByXmlId(bookXmlId, pageXmlId);

            if (bookPage == null)
            {
                string message = string.Format("Page not found for bookXmlId: '{0}' and page xmlId: '{1}'", bookXmlId, pageXmlId);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            PageBookmark bookmark = new PageBookmark
            {
                PageXmlId = pageXmlId,
                User = user,
                PagePosition = bookPage.Position,
                Book = m_bookRepository.FindBookByGuid(bookXmlId),
            };

            m_favoritesRepository.Save(bookmark);
        }

        public void RemovePageBookmark(string bookXmlId, string pageXmlId, string userName)
        {
            m_favoritesRepository.DeletePageBookmarkByPageXmlId(bookXmlId, pageXmlId, userName);
        }

        public IList<HeadwordBookmarkContract> GetHeadwordBookmarks(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return new List<HeadwordBookmarkContract>();

            var headwordResults = m_favoritesRepository.GetAllHeadwordBookmarks(userName);
            return Mapper.Map<IList<HeadwordBookmarkContract>>(headwordResults);
        }

        public void AddHeadwordBookmark(string bookXmlId, string entryXmlId, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = "Username is empty, cannot add bookmark";

                if (m_log.IsWarnEnabled)
                    m_log.Warn(message);
                throw new ArgumentException(message);
            }

            User user = m_userRepository.FindByUserName(userName);

            if (user == null)
            {
                string message = string.Format("Cannot locate user by username: '{0}'", userName);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var bookmark = new HeadwordBookmark
            {
                Book = m_bookRepository.FindBookByGuid(bookXmlId),
                User = user,
                XmlEntryId = entryXmlId
            };
            
            m_favoritesRepository.Save(bookmark);
        }

        public void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId, string userName)
        {
            m_favoritesRepository.DeleteHeadwordBookmark(bookXmlId, entryXmlId, userName);
        }
    }
}