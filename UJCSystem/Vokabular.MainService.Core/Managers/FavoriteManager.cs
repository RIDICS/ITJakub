using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoMapper;
using log4net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Types.Favorite;

namespace Vokabular.MainService.Core.Managers
{
    public class FavoriteManager
    {
        private readonly UserManager m_userManager;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly ResourceRepository m_resourceRepository;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FavoriteManager(UserManager userManager, CatalogValueRepository catalogValueRepository, ResourceRepository resourceRepository, FavoritesRepository favoritesRepository)
        {
            m_userManager = userManager;
            m_catalogValueRepository = catalogValueRepository;
            m_favoritesRepository = favoritesRepository;
            m_resourceRepository = resourceRepository;
        }

        private User TryGetUser()
        {
            return m_userManager.GetCurrentUser();
        }

        //private User TryGetUser()
        //{
        //    User user = m_userManager.GetCurrentUser();
            
        //    if (user == null)
        //    {
        //        string message = string.Format("Cannot locate user by username: '{0}'", m_userManager.GetCurrentUserName());
        //        if (m_log.IsErrorEnabled)
        //            m_log.Error(message);
        //        throw new ArgumentException(message);
        //    }

        //    if (user.UserName == m_defaultUserProvider.GetDefaultUserName())
        //    {
        //        string message = "Unregistered user, cannot execute specified action.";
        //        if (m_log.IsWarnEnabled)
        //            m_log.Warn(message);
        //        throw new ArgumentException(message);
        //    }

        //    return user;
        //}

        private void CheckItemOwnership(long itemOwnerUserId, User user)
        {
            if (user.Id != itemOwnerUserId)
            {
                throw new HttpErrorCodeException(
                    $"Current user ({user.UserName}) doesn't have permission manipulate with specified item owned by user with ID={itemOwnerUserId}", HttpStatusCode.Forbidden);
            }
        }

        public long CreatePageBookmark(CreateFavoritePageContract data)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var bookPage = m_resourceRepository.GetLatestResourceVersion<PageResource>(data.PageId);

            if (bookPage == null)
            {
                string message = $"Page with ID={data.PageId} not found";
                if (m_log.IsErrorEnabled)
                    m_log.Error(message);
                throw new HttpErrorCodeException(message, HttpStatusCode.BadRequest);
            }

            var label = GetFavoriteLabelAndCheckAuthorization(data.FavoriteLabelId, user.Id);
            label.LastUseTime = now;
            
            var favoriteItem = new FavoritePage
            {
                ResourcePage = bookPage.Resource,
                Title = data.Title,
                FavoriteLabel = label,
                CreateTime = now,
            };
            
            var resultId = (long) m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }
        
        //public IList<HeadwordBookmarkContract> GetHeadwordBookmarks()
        //{
        //    var userName = m_userManager.GetCurrentUserName();
        //    if (string.IsNullOrWhiteSpace(userName))
        //        return new List<HeadwordBookmarkContract>();

        //    var headwordResults = m_favoritesRepository.GetAllHeadwordBookmarks(userName);
        //    return Mapper.Map<IList<HeadwordBookmarkContract>>(headwordResults);
        //}

        //public void AddHeadwordBookmark(string bookXmlId, string entryXmlId)
        //{
        //    var now = DateTime.UtcNow;
        //    var user = TryGetUser();
        //    var defaultFavoriteLabel = m_favoritesRepository.GetDefaultFavoriteLabel(user.Id);

        //    var bookmark = new HeadwordBookmark
        //    {
        //        Book = m_bookRepository.FindBookByGuid(bookXmlId),
        //        User = user,
        //        XmlEntryId = entryXmlId,
        //        CreateTime = now,
        //        FavoriteLabel = defaultFavoriteLabel
        //    };
            
        //    m_favoritesRepository.Save(bookmark);
        //}

        //public void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId)
        //{
        //    var userName = m_userManager.GetCurrentUserName();
        //    m_favoritesRepository.DeleteHeadwordBookmark(bookXmlId, entryXmlId, userName);
        //}

        public List<FavoriteBookGroupedContract> GetFavoriteLabeledBooks(IList<long> projectIds)
        {
            if (projectIds == null)
            {
                projectIds = new List<long>();
            }
            var user = TryGetUser();
            var dbResult = m_favoritesRepository.GetFavoriteLabeledBooks(projectIds, user.Id);

            var resultList = new List<FavoriteBookGroupedContract>();
            foreach (var favoriteBookGroup in dbResult.GroupBy(x => x.Project.Id))
            {
                var favoriteItems = new FavoriteBookGroupedContract
                {
                    Id = favoriteBookGroup.Key,
                    FavoriteInfo = favoriteBookGroup.Select(Mapper.Map<FavoriteBaseWithLabelContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        public List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories(IList<int> categoryIds)
        {
            if (categoryIds == null)
            {
                categoryIds = new List<int>();
            }
            var user = TryGetUser();
            var dbResult = m_favoritesRepository.GetFavoriteLabeledCategories(categoryIds, user.Id);

            var resultList = new List<FavoriteCategoryGroupedContract>();
            foreach (var favoriteCategoryGroup in dbResult.GroupBy(x => x.Category.Id))
            {
                var favoriteItems = new FavoriteCategoryGroupedContract
                {
                    Id = favoriteCategoryGroup.Key,
                    FavoriteInfo = favoriteCategoryGroup.Select(Mapper.Map<FavoriteBaseWithLabelContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        private FavoriteLabel GetFavoriteLabelAndCheckAuthorization(long? labelId, int userId)
        {
            if (labelId == null)
            {
                var defaultLabel = m_favoritesRepository.GetDefaultFavoriteLabel(userId);
                return defaultLabel;
            }

            var label = m_favoritesRepository.FindById<FavoriteLabel>(labelId.Value);

            if (label == null)
            {
                throw new HttpErrorCodeException("FavoriteLabel not found", HttpStatusCode.BadRequest);
            }

            if (label.User.Id != userId)
            {
                throw new HttpErrorCodeException("Current user doesn't own this FavoriteLabel", HttpStatusCode.Forbidden);
            }

            return label;
        }

        public long CreateFavoriteBook(long projectId, string title, long labelId)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var project = m_favoritesRepository.Load<Project>(projectId);

            var label = GetFavoriteLabelAndCheckAuthorization(labelId, user.Id);
            label.LastUseTime = now;

            var favoriteItem = new FavoriteProject
            {
                Project = project,
                CreateTime = now,
                FavoriteLabel = label,
                Title = title
            };
            
            var resultId = (long) m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }

        public long CreateFavoriteCategory(int categoryId, string title, long labelId)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            var category = m_favoritesRepository.Load<Category>(categoryId);

            var label = GetFavoriteLabelAndCheckAuthorization(labelId, user.Id);
            
            label.LastUseTime = now;

            var favoriteItem = new FavoriteCategory
            {
                Category = category,
                CreateTime = now,
                FavoriteLabel = label,
                Title = title
            };
            
            var resultId = (long) m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }

        public long CreateFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query, string title, long labelId)
        {
            var now = DateTime.UtcNow;
            var user = TryGetUser();
            
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var bookTypeEntity = m_catalogValueRepository.GetBookType(bookTypeEnum);
            var label = GetFavoriteLabelAndCheckAuthorization(labelId, user.Id);

            label.LastUseTime = now;

            var favoriteItem = new FavoriteQuery
            {
                BookType = bookTypeEntity,
                Query = query,
                QueryType = queryTypeEnum,
                CreateTime = now,
                FavoriteLabel = label,
                Title = title,
            };
            
            var resultId = (long) m_favoritesRepository.Create(favoriteItem);
            return resultId;
        }

        public List<FavoriteLabelContract> GetFavoriteLabels(int? latestLabelCount)
        {
            var user = TryGetUser();

            var dbResult = latestLabelCount == null
                ? m_favoritesRepository.GetAllFavoriteLabels(user.Id)
                : m_favoritesRepository.GetLatestFavoriteLabels(latestLabelCount.Value, user.Id);

            return Mapper.Map<List<FavoriteLabelContract>>(dbResult);
        }

        public List<FavoriteBaseInfoContract> GetFavoriteItems(long? labelId, FavoriteTypeEnumContract? filterByType, string filterByTitle, FavoriteSortEnumContract sort, int start, int count)
        {
            var user = TryGetUser();
            var typeFilter = Mapper.Map<FavoriteTypeEnum?>(filterByType);

            var dbResult = m_favoritesRepository.GetFavoriteItems(labelId, typeFilter, filterByTitle, sort, start, count, user.Id);

            return Mapper.Map<List<FavoriteBaseInfoContract>>(dbResult);
        }

        public int GetFavoriteItemsCount(long? labelId, FavoriteTypeEnumContract? filterByType, string filterByTitle)
        {
            var user = TryGetUser();
            var typeFilter = Mapper.Map<FavoriteTypeEnum?>(filterByType);

            var resultCount = m_favoritesRepository.GetFavoriteItemsCount(labelId, typeFilter, filterByTitle, user.Id);
            return resultCount;
        }

        public List<FavoriteQueryContract> GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle, int start, int count)
        {
            var user = TryGetUser();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var dbResult = m_favoritesRepository.GetFavoriteQueries(labelId, bookTypeEnum, queryTypeEnum, filterByTitle, start, count, user.Id);

            return Mapper.Map<List<FavoriteQueryContract>>(dbResult);
        }

        public int GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            var user = TryGetUser();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = Mapper.Map<QueryTypeEnum>(queryType);

            var resultCount = m_favoritesRepository.GetFavoriteQueriesCount(labelId, bookTypeEnum, queryTypeEnum, filterByTitle, user.Id);
            return resultCount;
        }

        public List<FavoritePageContract> GetPageBookmarks(long projectId)
        {
            var user = TryGetUser();
            
            var allBookmarks = m_favoritesRepository.GetAllPageBookmarksByBookId(projectId, user.Id);

            return Mapper.Map<List<FavoritePageContract>>(allBookmarks);
        }

        private FavoriteLabelWithBooksAndCategories CreateFavoriteLabelWithBooksAndCategories(FavoriteLabel favoriteLabelEntity)
        {
            return new FavoriteLabelWithBooksAndCategories
            {
                ProjectIdList = new List<long>(),
                CategoryIdList = new List<int>(),
                Id = favoriteLabelEntity.Id,
                Name = favoriteLabelEntity.Name,
                Color = favoriteLabelEntity.Color
            };
        }

        public List<FavoriteLabelWithBooksAndCategories> GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
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

                favoriteLabel.ProjectIdList.Add(favoriteBook.Project.Id);
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
                throw new HttpErrorCodeException("User can't modify default favorite label", HttpStatusCode.BadRequest);

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
                throw new HttpErrorCodeException("Can't remove default favorite label", HttpStatusCode.BadRequest);

            m_favoritesRepository.Delete(favoriteLabel);
        }

        public void UpdateFavoriteItem(long id, string title)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.GetFavoriteItem(id);

            CheckItemOwnership(favoriteItem.FavoriteLabel.User.Id, user);

            favoriteItem.Title = title;
            
            m_favoritesRepository.Update(favoriteItem);
        }

        public void DeleteFavoriteItem(long id)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.GetFavoriteItem(id);

            CheckItemOwnership(favoriteItem.FavoriteLabel.User.Id, user);

            m_favoritesRepository.Delete(favoriteItem);
        }

        public FavoriteFullInfoContract GetFavoriteItem(long id)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.GetFavoriteItem(id);

            CheckItemOwnership(favoriteItem.FavoriteLabel.User.Id, user);

            var result = Mapper.Map<FavoriteFullInfoContract>(favoriteItem);
            switch (favoriteItem.FavoriteTypeEnum)
            {
                case FavoriteTypeEnum.Project:
                    var favoriteBook = (FavoriteProject) favoriteItem;
                    result.ProjectId = favoriteBook.Project.Id;
                    break;
                case FavoriteTypeEnum.Category:
                    var favoriteCategory = (FavoriteCategory) favoriteItem;
                    result.CategoryId = favoriteCategory.Category.Id;
                    break;
                case FavoriteTypeEnum.Page:
                    var favoritePageBookmark = (FavoritePage) favoriteItem;
                    result.PageId = favoritePageBookmark.ResourcePage.Id;
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