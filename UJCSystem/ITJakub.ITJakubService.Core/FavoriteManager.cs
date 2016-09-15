using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;
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

        private void CheckItemOwnership(long itemOwnerUserId, User user)
        {
            if (user.Id != itemOwnerUserId)
                throw new ArgumentException(string.Format("Current user ({0}) doesn't have permission manipulate with specified item owned by user with ID={1}", user.UserName, itemOwnerUserId));
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
                    FavoriteInfo = favoriteBookGroup.Select(Mapper.Map<FavoriteBaseDetailContract>).ToList()
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
                    FavoriteInfo = favoriteCategoryGroup.Select(Mapper.Map<FavoriteBaseDetailContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        private FavoriteLabel GetFavoriteLabelAndCheckAuthorization(long? labelId, int userId)
        {
            var label = labelId == null
                ? m_favoritesRepository.GetDefaultFavoriteLabel(userId)
                : m_favoritesRepository.FindById<FavoriteLabel>(labelId.Value);

            if (label.User.Id != label.Id)
            {
                throw new UnauthorizedAccessException();
            }

            return label;
        }

        public void CreateFavoriteBook(long bookId, string title, long? labelId, string userName)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser(userName);
            var book = m_favoritesRepository.Load<Book>(bookId);

            var label = GetFavoriteLabelAndCheckAuthorization(labelId, user.Id);
            
            var favoriteItem = new FavoriteBook
            {
                Book = book,
                CreateTime = now,
                User = user,
                FavoriteLabel = m_favoritesRepository.Load<FavoriteLabel>(label.Id),
                Title = title
            };

            m_favoritesRepository.Create(favoriteItem);
        }

        public void CreateFavoriteCategory(int categoryId, string title, long? labelId, string userName)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser(userName);
            var category = m_favoritesRepository.Load<Category>(categoryId);

            var label = GetFavoriteLabelAndCheckAuthorization(labelId, user.Id);
            
            var favoriteItem = new FavoriteCategory
            {
                Category = category,
                CreateTime = now,
                User = user,
                FavoriteLabel = m_favoritesRepository.Load<FavoriteLabel>(label.Id),
                Title = title
            };

            m_favoritesRepository.Create(favoriteItem);
        }

        public void CreateFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query, string title, long? labelId, string userName)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser(userName);
            var label = GetFavoriteLabelAndCheckAuthorization(labelId, user.Id);

            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var bookTypeEntity = m_bookVersionRepository.GetBookType(bookTypeEnum);
            
            var favoriteItem = new FavoriteQuery
            {
                BookType = bookTypeEntity,
                Query = query,
                QueryType = queryTypeEnum,
                CreateTime = now,
                User = user,
                FavoriteLabel = m_favoritesRepository.Load<FavoriteLabel>(label.Id),
                Title = title,
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

        public IList<FavoriteBaseInfoContract> GetFavoriteItems(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle, FavoriteSortContract sort, string userName)
        {
            var user = TryGetUser(userName);
            var typeFilter = Mapper.Map<FavoriteTypeEnum?>(filterByType);

            var dbResult = m_favoritesRepository.GetFavoriteItems(labelId, typeFilter, filterByTitle, sort, user.Id);

            return Mapper.Map<IList<FavoriteBaseInfoContract>>(dbResult);
        }

        public IList<FavoriteQueryContract> GetFavoriteQueries(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string userName)
        {
            var user = TryGetUser(userName);
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var dbResult = m_favoritesRepository.GetFavoriteQueries(bookTypeEnum, queryTypeEnum, user.Id);

            return Mapper.Map<IList<FavoriteQueryContract>>(dbResult);
        }

        public long CreateFavoriteLabel(string name, string color, string userName, bool isDefault)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser(userName);
            var favoriteLabel = new FavoriteLabel
            {
                Name = name,
                Color = color,
                IsDefault = isDefault,
                LastUseTime = now,
                User = m_favoritesRepository.Load<User>(user.Id)
            };

            var id = m_favoritesRepository.Create(favoriteLabel);
            return (long) id;
        }

        public void UpdateFavoriteLabel(long labelId, string name, string color, string userName)
        {
            var user = TryGetUser(userName);
            var favoriteLabel = m_favoritesRepository.FindById<FavoriteLabel>(labelId);

            CheckItemOwnership(favoriteLabel.User.Id, user);
            
            if (favoriteLabel.IsDefault)
                throw new ArgumentException("User can't modify default favorite label");

            favoriteLabel.Name = name;
            favoriteLabel.Color = color;

            m_favoritesRepository.Update(favoriteLabel);
        }

        public void DeleteFavoriteLabel(long labelId, string userName)
        {
            var user = TryGetUser(userName);
            var favoriteLabel = m_favoritesRepository.FindById<FavoriteLabel>(labelId);

            CheckItemOwnership(favoriteLabel.User.Id, user);

            if (favoriteLabel.IsDefault)
                throw new ArgumentException("Can't remove default favorite label");

            m_favoritesRepository.Delete(favoriteLabel);
        }

        public void UpdateFavoriteItem(long id, string title, string userName)
        {
            var user = TryGetUser(userName);
            var favoriteItem = m_favoritesRepository.FindById<FavoriteBase>(id);

            CheckItemOwnership(favoriteItem.User.Id, user);

            favoriteItem.Title = title;
            
            m_favoritesRepository.Update(favoriteItem);
        }

        public void DeleteFavoriteItem(long id, string userName)
        {
            var user = TryGetUser(userName);
            var favoriteItem = m_favoritesRepository.FindById<FavoriteBase>(id);

            CheckItemOwnership(favoriteItem.User.Id, user);

            m_favoritesRepository.Delete(favoriteItem);
        }
    }
}