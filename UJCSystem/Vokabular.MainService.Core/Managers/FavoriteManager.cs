using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Favorite;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Favorite;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Types.Favorite;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class FavoriteManager
    {
        private readonly AuthenticationManager m_authenticationManager;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly FavoritesRepository m_favoritesRepository;
        private readonly IMapper m_mapper;
        private readonly ResourceRepository m_resourceRepository;

        //private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FavoriteManager(AuthenticationManager authenticationManager, CatalogValueRepository catalogValueRepository,
            ResourceRepository resourceRepository, FavoritesRepository favoritesRepository, IMapper mapper)
        {
            m_authenticationManager = authenticationManager;
            m_catalogValueRepository = catalogValueRepository;
            m_favoritesRepository = favoritesRepository;
            m_mapper = mapper;
            m_resourceRepository = resourceRepository;
        }

        private User TryGetUser()
        {
            return m_authenticationManager.GetCurrentUser();
        }

        public long CreateFavoritePage(CreateFavoritePageContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateFavoritePageWork(m_favoritesRepository, m_resourceRepository, data, userId);
            var resultId = work.Execute();
            return resultId;
        }
        
        //public IList<HeadwordBookmarkContract> GetHeadwordBookmarks()
        //{
        //    var userName = m_userManager.GetCurrentUserName();
        //    if (string.IsNullOrWhiteSpace(userName))
        //        return new List<HeadwordBookmarkContract>();

        //    var headwordResults = m_favoritesRepository.GetAllHeadwordBookmarks(userName);
        //    return m_mapper.Map<IList<HeadwordBookmarkContract>>(headwordResults);
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

        public List<FavoriteBookGroupedContract> GetFavoriteLabeledBooks(IList<long> projectIds, BookTypeEnumContract? bookType)
        {
            if (projectIds == null)
            {
                projectIds = new List<long>();
            }
            var user = TryGetUser();
            var bookTypeEnum = m_mapper.Map<BookTypeEnum?>(bookType);
            var dbResult = m_favoritesRepository.InvokeUnitOfWork(x => x.GetFavoriteLabeledBooks(projectIds, bookTypeEnum, user.Id));

            var resultList = new List<FavoriteBookGroupedContract>();
            foreach (var favoriteBookGroup in dbResult.GroupBy(x => x.Project.Id))
            {
                var favoriteItems = new FavoriteBookGroupedContract
                {
                    Id = favoriteBookGroup.Key,
                    FavoriteInfo = favoriteBookGroup.Select(m_mapper.Map<FavoriteBaseWithLabelContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        public List<FavoriteCategoryGroupedContract> GetFavoriteLabeledCategories()
        {
            var user = TryGetUser();
            var dbResult = m_favoritesRepository.InvokeUnitOfWork(x => x.GetFavoriteLabeledCategories(user.Id));

            var resultList = new List<FavoriteCategoryGroupedContract>();
            foreach (var favoriteCategoryGroup in dbResult.GroupBy(x => x.Category.Id))
            {
                var favoriteItems = new FavoriteCategoryGroupedContract
                {
                    Id = favoriteCategoryGroup.Key,
                    FavoriteInfo = favoriteCategoryGroup.Select(m_mapper.Map<FavoriteBaseWithLabelContract>).ToList()
                };
                resultList.Add(favoriteItems);
            }
            return resultList;
        }

        public long CreateFavoriteProject(CreateFavoriteProjectContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateFavoriteProjectWork(m_favoritesRepository, data, userId);
            var resultId = work.Execute();
            return resultId;
        }

        public long CreateFavoriteCategory(CreateFavoriteCategoryContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateFavoriteCategoryWork(m_favoritesRepository, data, userId);
            var resultId = work.Execute();
            return resultId;
        }

        public long CreateFavoriteQuery(CreateFavoriteQueryContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateFavoriteQueryWork(m_favoritesRepository, m_catalogValueRepository, data, userId, m_mapper);
            var resultId = work.Execute();
            return resultId;
        }

        public List<FavoriteLabelContract> GetFavoriteLabels(int? latestLabelCount)
        {
            var user = TryGetUser();

            var dbResult = m_favoritesRepository.InvokeUnitOfWork(repository =>
            {
                return latestLabelCount == null
                    ? repository.GetAllFavoriteLabels(user.Id)
                    : repository.GetLatestFavoriteLabels(latestLabelCount.Value, user.Id);
            });

            return m_mapper.Map<List<FavoriteLabelContract>>(dbResult);
        }

        public PagedResultList<FavoriteBaseInfoContract> GetFavoriteItems(int? start, int? count, long? labelId, FavoriteTypeEnumContract? filterByType, string filterByTitle, FavoriteSortEnumContract sort)
        {
            var user = TryGetUser();
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var typeFilter = m_mapper.Map<FavoriteTypeEnum?>(filterByType);

            var dbResult = m_favoritesRepository.InvokeUnitOfWork(x => x.GetFavoriteItems(labelId, typeFilter, filterByTitle, sort, startValue, countValue, user.Id));
            
            return new PagedResultList<FavoriteBaseInfoContract>
            {
                List = m_mapper.Map<List<FavoriteBaseInfoContract>>(dbResult.List),
                TotalCount = dbResult.Count
            };
        }

        public PagedResultList<FavoriteQueryContract> GetFavoriteQueries(int? start, int? count, long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            var user = TryGetUser();
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);
            var bookTypeEnum = m_mapper.Map<BookTypeEnum>(bookType);
            var queryTypeEnum = m_mapper.Map<QueryTypeEnum>(queryType);

            var dbResult = m_favoritesRepository.InvokeUnitOfWork(x => x.GetFavoriteQueries(labelId, bookTypeEnum, queryTypeEnum, filterByTitle, startValue, countValue, user.Id));

            return new PagedResultList<FavoriteQueryContract>
            {
                List = m_mapper.Map<List<FavoriteQueryContract>>(dbResult.List),
                TotalCount = dbResult.Count,
            };
        }

        public List<FavoritePageContract> GetPageBookmarks(long projectId)
        {
            var user = TryGetUser();
            
            var allBookmarks = m_favoritesRepository.InvokeUnitOfWork(x => x.GetAllPageBookmarksByBookId(projectId, user.Id));

            return m_mapper.Map<List<FavoritePageContract>>(allBookmarks);
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
            IList<FavoriteProject> booksDbResult = null;
            IList<FavoriteCategory> categoriesDbResult = null;
            var favoriteLabels = new Dictionary<long, FavoriteLabelWithBooksAndCategories>();

            var user = TryGetUser();
            var bookTypeEnum = m_mapper.Map<BookTypeEnum>(bookType);
            
            m_favoritesRepository.InvokeUnitOfWork(repository =>
            {
                booksDbResult = repository.GetFavoriteBooksWithLabel(bookTypeEnum, user.Id);
                categoriesDbResult = repository.GetFavoriteCategoriesWithLabel(user.Id);
            });

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

        public long CreateFavoriteLabel(FavoriteLabelContractBase data, bool isDefault = false)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var resultId = new CreateOrUpdateFavoriteLabelWork(m_favoritesRepository, data, userId, null, isDefault).Execute();
            return resultId;
        }

        public void UpdateFavoriteLabel(long labelId, FavoriteLabelContractBase data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new CreateOrUpdateFavoriteLabelWork(m_favoritesRepository, data, userId, labelId).Execute();
        }

        public void DeleteFavoriteLabel(long labelId)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new DeleteFavoriteLabelWork(m_favoritesRepository, labelId, userId).Execute();
        }

        public void UpdateFavoriteItem(long id, UpdateFavoriteContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new UpdateFavoriteItemWork(m_favoritesRepository, id, data, userId).Execute();
        }

        public void DeleteFavoriteItem(long id)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new DeleteFavoriteItemWork(m_favoritesRepository, id, userId).Execute();
        }

        public FavoriteFullInfoContract GetFavoriteItem(long id)
        {
            var user = TryGetUser();
            var favoriteItem = m_favoritesRepository.InvokeUnitOfWork(x => x.GetFavoriteItem(id));

            if (favoriteItem == null)
            {
                throw new MainServiceException(MainServiceErrorCode.ItemNotFound, "Item not found", HttpStatusCode.NotFound);
            }

            OwnershipHelper.CheckItemOwnership(favoriteItem.FavoriteLabel.User.Id, user.Id);

            var result = m_mapper.Map<FavoriteFullInfoContract>(favoriteItem);
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
                    var resourcePage = m_favoritesRepository.InvokeUnitOfWork(x => x.FindById<Resource>(favoritePageBookmark.ResourcePage.Id));
                    result.PageId = favoritePageBookmark.ResourcePage.Id;
                    result.ProjectId = resourcePage.Project.Id;
                    break;
                case FavoriteTypeEnum.Query:
                    var favoriteQuery = (FavoriteQuery) favoriteItem;
                    var bookType = m_favoritesRepository.InvokeUnitOfWork(x => x.FindById<BookType>(favoriteQuery.BookType.Id));
                    result.BookType = m_mapper.Map<BookTypeEnumContract>(bookType.Type);
                    result.QueryType = m_mapper.Map<QueryTypeEnumContract>(favoriteQuery.QueryType);
                    result.Query = favoriteQuery.Query;
                    break;
            }
            return result;
        }
    }
}