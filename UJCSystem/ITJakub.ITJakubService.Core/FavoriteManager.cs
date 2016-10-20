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
        private readonly UserManager m_userManager;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly BookRepository m_bookRepository;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FavoriteManager(UserManager userManager, DefaultUserProvider defaultUserProvider, FavoritesRepository favoritesRepository, BookRepository bookRepository, BookVersionRepository bookVersionRepository)
        {
            m_userManager = userManager;
            m_defaultUserProvider = defaultUserProvider;
            m_favoritesRepository = favoritesRepository;
            m_bookRepository = bookRepository;
            m_bookVersionRepository = bookVersionRepository;
        }
        
        private User TryGetUser()
        {
            User user = m_userManager.GetCurrentUser();
            
            if (user == null)
            {
                string message = string.Format("Cannot locate user by username: '{0}'", m_userManager.GetCurrentUserName());
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            if (user.UserName == m_defaultUserProvider.GetDefaultUserName())
            {
                string message = "Unregistered user, cannot execute specified action.";
                if (m_log.IsWarnEnabled)
                    m_log.Warn(message);
                throw new ArgumentException(message);
            }

            return user;
        }

        private void CheckItemOwnership(long itemOwnerUserId, User user)
        {
            if (user.Id != itemOwnerUserId)
                throw new ArgumentException(string.Format("Current user ({0}) doesn't have permission manipulate with specified item owned by user with ID={1}", user.UserName, itemOwnerUserId));
        }

        public long CreatePageBookmark(string bookXmlId, string pageXmlId, string title, long? labelId)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var bookPage = m_bookVersionRepository.GetPageByXmlId(bookXmlId, pageXmlId);

            if (bookPage == null)
            {
                string message = string.Format("Page not found for bookXmlId: '{0}' and page xmlId: '{1}'", bookXmlId, pageXmlId);
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new ArgumentException(message);
            }

            var labelIds = new List<long>();
            if (labelId != null)
            {
                labelIds.Add(labelId.Value);
            }

            Book book = m_bookVersionRepository.Load<Book>(bookPage.BookVersion.Book.Id);
            FavoriteLabel favoriteLabel = GetFavoriteLabelsAndCheckAuthorization(labelIds, user.Id).First();
            favoriteLabel.LastUseTime = now;

            PageBookmark bookmark = new PageBookmark
            {
                PageXmlId = pageXmlId,
                User = user,
                PagePosition = bookPage.Position,
                Book = book,
                Title = title,
                FavoriteLabel = favoriteLabel,
                CreateTime = now,
            };

            return (long) m_favoritesRepository.Create(bookmark);
        }
        
        public IList<HeadwordBookmarkContract> GetHeadwordBookmarks()
        {
            var userName = m_userManager.GetCurrentUserName();
            if (string.IsNullOrWhiteSpace(userName))
                return new List<HeadwordBookmarkContract>();

            var headwordResults = m_favoritesRepository.GetAllHeadwordBookmarks(userName);
            return Mapper.Map<IList<HeadwordBookmarkContract>>(headwordResults);
        }

        public void AddHeadwordBookmark(string bookXmlId, string entryXmlId)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var defaultFavoriteLabel = m_favoritesRepository.GetDefaultFavoriteLabel(user.Id);

            var bookmark = new HeadwordBookmark
            {
                Book = m_bookRepository.FindBookByGuid(bookXmlId),
                User = user,
                XmlEntryId = entryXmlId,
                CreateTime = now,
                FavoriteLabel = defaultFavoriteLabel
            };
            
            m_favoritesRepository.Save(bookmark);
        }

        public void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId)
        {
            var userName = m_userManager.GetCurrentUserName();
            m_favoritesRepository.DeleteHeadwordBookmark(bookXmlId, entryXmlId, userName);
        }

        public IList<FavoriteBookInfoContract> GetFavoriteLabeledBooks(IList<long> bookIds)
        {
            if (bookIds == null)
            {
                bookIds = new List<long>();
            }
            var user = TryGetUser();
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

        public IList<FavoriteCategoryContract> GetFavoriteLabeledCategories(IList<int> categoryIds)
        {
            if (categoryIds == null)
            {
                categoryIds = new List<int>();
            }
            var user = TryGetUser();
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

        private IList<FavoriteLabel> GetFavoriteLabelsAndCheckAuthorization(IList<long> labelIds, int userId)
        {
            if (labelIds == null || labelIds.Count == 0)
            {
                var defaultLabel = m_favoritesRepository.GetDefaultFavoriteLabel(userId);
                return new List<FavoriteLabel> {defaultLabel};
            }

            var labels = m_favoritesRepository.GetFavoriteLabelsById(labelIds);
            if (labels.Any(x => x.User.Id != userId))
            {
                throw new UnauthorizedAccessException();
            }

            if (labels.Count != labelIds.Count)
            {
                throw new ArgumentException("All specified labels were not found");
            }

            return labels;
        }

        public IList<long> CreateFavoriteBook(long bookId, string title, IList<long> labelIds)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var book = m_favoritesRepository.Load<Book>(bookId);

            var labels = GetFavoriteLabelsAndCheckAuthorization(labelIds, user.Id);
            var labelsDictionary = labels.ToDictionary(x => x.Id);
            var itemsToSave = new List<FavoriteBook>();

            foreach (var labelId in labelIds)
            {
                var label = labelsDictionary[labelId];
                label.LastUseTime = now;

                var favoriteItem = new FavoriteBook
                {
                    Book = book,
                    CreateTime = now,
                    User = user,
                    FavoriteLabel = label,
                    Title = title
                };
                itemsToSave.Add(favoriteItem);
            }

            var result = m_favoritesRepository.CreateAll(itemsToSave);
            return result.Cast<long>().ToList();
        }

        public IList<long> CreateFavoriteCategory(int categoryId, string title, IList<long> labelIds)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var category = m_favoritesRepository.Load<Category>(categoryId);

            var labels = GetFavoriteLabelsAndCheckAuthorization(labelIds, user.Id);
            var labelsDictionary = labels.ToDictionary(x => x.Id);
            var itemsToSave = new List<FavoriteCategory>();

            foreach (var labelId in labelIds)
            {
                var label = labelsDictionary[labelId];
                label.LastUseTime = now;

                var favoriteItem = new FavoriteCategory
                {
                    Category = category,
                    CreateTime = now,
                    User = user,
                    FavoriteLabel = label,
                    Title = title
                };
                itemsToSave.Add(favoriteItem);
            }

            var result = m_favoritesRepository.CreateAll(itemsToSave);
            return result.Cast<long>().ToList();
        }

        public IList<long> CreateFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query, string title, IList<long> labelIds)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();

            var labels = GetFavoriteLabelsAndCheckAuthorization(labelIds, user.Id);
            var labelsDictionary = labels.ToDictionary(x => x.Id);
            var itemsToSave = new List<FavoriteQuery>();

            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);
            var bookTypeEntity = m_bookVersionRepository.GetBookType(bookTypeEnum);

            foreach (var labelId in labelIds)
            {
                var label = labelsDictionary[labelId];
                label.LastUseTime = now;

                var favoriteItem = new FavoriteQuery
                {
                    BookType = bookTypeEntity,
                    Query = query,
                    QueryType = queryTypeEnum,
                    CreateTime = now,
                    User = user,
                    FavoriteLabel = label,
                    Title = title,
                };
                itemsToSave.Add(favoriteItem);
            }

            var result = m_favoritesRepository.CreateAll(itemsToSave);
            return result.Cast<long>().ToList();
        }

        public IList<FavoriteLabelContract> GetFavoriteLabels(int latestLabelCount)
        {
            var user = TryGetUser();

            var dbResult = latestLabelCount == 0
                ? m_favoritesRepository.GetAllFavoriteLabels(user.Id)
                : m_favoritesRepository.GetLatestFavoriteLabels(latestLabelCount, user.Id);

            return Mapper.Map<IList<FavoriteLabelContract>>(dbResult);
        }

        public IList<FavoriteBaseInfoContract> GetFavoriteItems(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle, FavoriteSortContract sort, int start, int count)
        {
            var user = TryGetUser();
            var typeFilter = Mapper.Map<FavoriteTypeEnum?>(filterByType);

            var dbResult = m_favoritesRepository.GetFavoriteItems(labelId, typeFilter, filterByTitle, sort, start, count, user.Id);

            return Mapper.Map<IList<FavoriteBaseInfoContract>>(dbResult);
        }

        public int GetFavoriteItemsCount(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle)
        {
            var user = TryGetUser();
            var typeFilter = Mapper.Map<FavoriteTypeEnum?>(filterByType);

            var resultCount = m_favoritesRepository.GetFavoriteItemsCount(labelId, typeFilter, filterByTitle, user.Id);
            return resultCount;
        }

        public IList<FavoriteQueryContract> GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle, int start, int count)
        {
            var user = TryGetUser();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var dbResult = m_favoritesRepository.GetFavoriteQueries(labelId, bookTypeEnum, queryTypeEnum, filterByTitle, start, count, user.Id);

            return Mapper.Map<IList<FavoriteQueryContract>>(dbResult);
        }

        public int GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            var user = TryGetUser();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var resultCount = m_favoritesRepository.GetFavoriteQueriesCount(labelId, bookTypeEnum, queryTypeEnum, filterByTitle, user.Id);
            return resultCount;
        }

        public List<PageBookmarkContract> GetPageBookmarks(string bookXmlId)
        {
            var user = TryGetUser();
            
            var allBookmarks = m_favoritesRepository.GetAllPageBookmarksByBookId(bookXmlId, user.Id);

            return Mapper.Map<List<PageBookmarkContract>>(allBookmarks);
        }

        private FavoriteLabelWithBooksAndCategories CreateFavoriteLabelWithBooksAndCategories(FavoriteLabel favoriteLabelEntity)
        {
            return new FavoriteLabelWithBooksAndCategories
            {
                BookIdList = new List<long>(),
                CategoryIdList = new List<int>(),
                Id = favoriteLabelEntity.Id,
                Name = favoriteLabelEntity.Name,
                Color = favoriteLabelEntity.Color
            };
        }

        public IList<FavoriteLabelWithBooksAndCategories> GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            var user = TryGetUser();

            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var booksDbResult = m_favoritesRepository.GetFavoriteBooksWithLabel(bookTypeEnum, user.Id);
            var categoriesDbResult = m_favoritesRepository.GetFavoriteCategoriesWithLabel(bookTypeEnum, user.Id);
            var favoriteLabels = new Dictionary<long, FavoriteLabelWithBooksAndCategories>();

            foreach (var favoriteBook in booksDbResult)
            {
                FavoriteLabelWithBooksAndCategories favoriteLabel;
                if (!favoriteLabels.TryGetValue(favoriteBook.FavoriteLabel.Id, out favoriteLabel))
                {
                    favoriteLabel = CreateFavoriteLabelWithBooksAndCategories(favoriteBook.FavoriteLabel);
                    favoriteLabels.Add(favoriteLabel.Id, favoriteLabel);
                }

                favoriteLabel.BookIdList.Add(favoriteBook.Book.Id);
            }
            foreach (var favoriteCategory in categoriesDbResult)
            {
                FavoriteLabelWithBooksAndCategories favoriteLabel;
                if (!favoriteLabels.TryGetValue(favoriteCategory.FavoriteLabel.Id, out favoriteLabel))
                {
                    favoriteLabel = CreateFavoriteLabelWithBooksAndCategories(favoriteCategory.FavoriteLabel);
                    favoriteLabels.Add(favoriteLabel.Id, favoriteLabel);
                }

                favoriteLabel.CategoryIdList.Add(favoriteCategory.Category.Id);
            }

            return favoriteLabels.Select(x => x.Value)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public long CreateFavoriteLabel(string name, string color, bool isDefault)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
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

        public void UpdateFavoriteLabel(long labelId, string name, string color)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var favoriteLabel = m_favoritesRepository.FindById<FavoriteLabel>(labelId);

            CheckItemOwnership(favoriteLabel.User.Id, user);
            
            if (favoriteLabel.IsDefault)
                throw new ArgumentException("User can't modify default favorite label");

            favoriteLabel.Name = name;
            favoriteLabel.Color = color;
            favoriteLabel.LastUseTime = now;

            m_favoritesRepository.Update(favoriteLabel);
        }

        public void DeleteFavoriteLabel(long labelId)
        {
            var user = TryGetUser();
            var favoriteLabel = m_favoritesRepository.FindById<FavoriteLabel>(labelId);

            CheckItemOwnership(favoriteLabel.User.Id, user);

            if (favoriteLabel.IsDefault)
                throw new ArgumentException("Can't remove default favorite label");

            m_favoritesRepository.Delete(favoriteLabel);
        }

        public void UpdateFavoriteItem(long id, string title)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.FindById<FavoriteBase>(id);

            CheckItemOwnership(favoriteItem.User.Id, user);

            favoriteItem.Title = title;
            
            m_favoritesRepository.Update(favoriteItem);
        }

        public void DeleteFavoriteItem(long id)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.FindById<FavoriteBase>(id);

            CheckItemOwnership(favoriteItem.User.Id, user);

            m_favoritesRepository.Delete(favoriteItem);
        }

        public FavoriteFullInfoContract GetFavoriteItem(long id)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.FindById<FavoriteBase>(id);

            CheckItemOwnership(favoriteItem.User.Id, user);

            var result = Mapper.Map<FavoriteFullInfoContract>(favoriteItem);
            switch (favoriteItem.FavoriteTypeEnum)
            {
                case FavoriteTypeEnum.Book:
                    var favoriteBook = (FavoriteBook) favoriteItem;
                    var book = m_favoritesRepository.FindById<Book>(favoriteBook.Book.Id);
                    result.Book = Mapper.Map<BookIdContract>(book);
                    break;
                case FavoriteTypeEnum.Category:
                    var favoriteCategory = (FavoriteCategory) favoriteItem;
                    var category = m_favoritesRepository.FindById<Category>(favoriteCategory.Category.Id);
                    result.Category = Mapper.Map<CategoryContract>(category);
                    break;
                case FavoriteTypeEnum.PageBookmark:
                    var favoritePageBookmark = (PageBookmark) favoriteItem;
                    var book2 = m_favoritesRepository.FindById<Book>(favoritePageBookmark.Book.Id);
                    result.Book = Mapper.Map<BookIdContract>(book2);
                    result.PageXmlId = favoritePageBookmark.PageXmlId;
                    result.PagePosition = favoritePageBookmark.PagePosition;
                    break;
                case FavoriteTypeEnum.Query:
                    var favoriteQuery = (FavoriteQuery) favoriteItem;
                    var bookType = m_favoritesRepository.FindById<BookType>(favoriteQuery.BookType.Id);
                    result.BookType = Mapper.Map<BookTypeEnumContract>(bookType.Type);
                    result.QueryType = Mapper.Map<QueryTypeEnumContract>(favoriteQuery.QueryType);
                    result.Query = favoriteQuery.Query;
                    break;
            }
            return result;
        }
    }
}