using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
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

        private User TryGetUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                string message = "Username is empty, cannot execute specified action.";

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

            return user;
        }

        public void AddPageBookmark(string bookXmlId, string pageXmlId, string userName)
        {
            var user = TryGetUser(userName);
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

        public bool SetPageBookmarkTitle(string bookXmlId, string pageXmlId, string title, string userName)
        {
            var user = TryGetUser(userName);
            var bookPage = m_bookVersionRepository.GetPageByXmlId(bookXmlId, pageXmlId);

            if (bookPage == null)
            {
                string message = string.Format("Page not found for bookXmlId: '{0}' and page xmlId: '{1}'", bookXmlId, pageXmlId);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var done = false;

            foreach (var pageBookmark in m_favoritesRepository.GetPageBookmarkByPageXmlId(bookXmlId, pageXmlId, userName))
            {
                pageBookmark.Title = title;

                m_favoritesRepository.Save(pageBookmark);

                done = true;
            }

            return done;
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
            var user = TryGetUser(userName);

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

        public IList<FavoriteBookInfoContract> GetFavoriteLabeledBooks(IList<long> bookIds, string userName)
        {
            var user = TryGetUser(userName);
            var dbResult = m_favoritesRepository.GetFavoriteLabeledBooks(bookIds, user.Id);

            var resultList = new List<FavoriteBookInfoContract>();
            foreach (var favoriteBookGroup in dbResult.GroupBy(x => x.Book.Id))
            {
                var favoriteItems = new FavoriteBookInfoContract
                {
                    Id = favoriteBookGroup.Key,
                    FavoriteInfo = favoriteBookGroup.Select(Mapper.Map<FavoriteBaseInfoContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        public IList<FavoriteCategoryContract> GetFavoriteLabeledCategories(IList<int> categoryIds, string userName)
        {
            var user = TryGetUser(userName);
            var dbResult = m_favoritesRepository.GetFavoriteLabeledCategories(categoryIds, user.Id);

            var resultList = new List<FavoriteCategoryContract>();
            foreach (var favoriteCategoryGroup in dbResult.GroupBy(x => x.Category.Id))
            {
                var favoriteItems = new FavoriteCategoryContract
                {
                    Id = favoriteCategoryGroup.Key,
                    FavoriteInfo = favoriteCategoryGroup.Select(Mapper.Map<FavoriteBaseInfoContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        public void CreateFavoriteBook(long bookId, string title, long? labelId, string userName)
        {
            var user = TryGetUser(userName);
            var book = m_favoritesRepository.Load<Book>(bookId);

            var label = labelId == null
                ? m_favoritesRepository.GetDefaultFavoriteLabel(user.Id)
                : m_favoritesRepository.FindById<FavoriteLabel>(labelId.Value);
            
            if (label.User.Id != label.Id)
            {
                throw new UnauthorizedAccessException();
            }

            var favoriteItem = new FavoriteBook
            {
                Book = book,
                User = user,
                FavoriteLabel = m_favoritesRepository.Load<FavoriteLabel>(label.Id),
                Title = title
            };

            m_favoritesRepository.Create(favoriteItem);
        }

        public void CreateFavoriteCategory(int categoryId, string title, long? labelId, string userName)
        {
            var user = TryGetUser(userName);
            var category = m_favoritesRepository.Load<Category>(categoryId);

            var label = labelId == null
                ? m_favoritesRepository.GetDefaultFavoriteLabel(user.Id)
                : m_favoritesRepository.FindById<FavoriteLabel>(labelId.Value);

            if (label.User.Id != label.Id)
            {
                throw new UnauthorizedAccessException();
            }

            var favoriteItem = new FavoriteCategory
            {
                Category = category,
                User = user,
                FavoriteLabel = m_favoritesRepository.Load<FavoriteLabel>(label.Id),
                Title = title
            };

            m_favoritesRepository.Create(favoriteItem);
        }

        public IList<FavoriteLabelContract> GetFavoriteLabels(int latestLabelCount, string userName)
        {
            var user = TryGetUser(userName);

            var dbResult = latestLabelCount == 0
                ? m_favoritesRepository.GetAllFavoriteLabels(user.Id)
                : m_favoritesRepository.GetLatestFavoriteLabels(latestLabelCount, user.Id);

            return Mapper.Map<IList<FavoriteLabelContract>>(dbResult);
        }
    }
}