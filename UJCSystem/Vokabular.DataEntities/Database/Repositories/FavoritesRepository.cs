using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataContracts.Types.Favorite;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class FavoritesRepository : MainDbRepositoryBase
    {
        public FavoritesRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual IList<FavoritePage> GetAllPageBookmarksByBookId(long projectId, long userId)
        {
            FavoritePage favoritePageAlias = null;
            FavoriteLabel favoriteLabelAlias = null;
            Resource resourcePageAlias = null;

            return GetSession().QueryOver(() => favoritePageAlias)
                .JoinAlias(() => favoritePageAlias.ResourcePage, () => resourcePageAlias)
                .JoinAlias(() => favoritePageAlias.FavoriteLabel, () => favoriteLabelAlias)
                .Where(() => resourcePageAlias.Project.Id == projectId && favoriteLabelAlias.User.Id == userId)
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .List();
        }
        
        
        public virtual IList<FavoriteHeadword> GetAllFavoriteHeadwords(long userId)
        {
            FavoriteLabel favoriteLabelAlias = null;

            return GetSession().QueryOver<FavoriteHeadword>()
                .JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .Where(() => favoriteLabelAlias.User.Id == userId)
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .OrderBy(x => x.Title).Asc
                .List();
        }

        public virtual IList<FavoriteLabel> GetFavoriteLabelsById(IList<long> labelIds)
        {
            return GetSession().QueryOver<FavoriteLabel>()
                .WhereRestrictionOn(x => x.Id).IsInG(labelIds)
                .List();
        }

        public virtual IList<FavoriteProject> GetFavoriteLabeledBooks(IList<long> projectIds, BookTypeEnum? bookType,
            ProjectTypeEnum? projectType, int userId)
        {
            FavoriteProject favoriteItemAlias = null;
            FavoriteLabel favoriteLabelAlias = null;
            Project projectAlias = null;
            Snapshot latestSnapshotAlias = null;
            BookType bookTypeAlias = null;

            var restriction = Restrictions.Disjunction();
            if (projectIds.Count > 0)
            {
                restriction.Add(Restrictions.InG(Projections.Property(() => projectAlias.Id), projectIds));
            }
            if (bookType != null)
            {
                restriction.Add(() => bookTypeAlias.Type == bookType.Value);
            }

            var query = GetSession().QueryOver(() => favoriteItemAlias)
                .JoinAlias(() => favoriteItemAlias.FavoriteLabel, () => favoriteLabelAlias)
                .JoinAlias(() => favoriteItemAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => latestSnapshotAlias)
                .JoinAlias(() => latestSnapshotAlias.BookTypes, () => bookTypeAlias)
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .Where(restriction)
                .And(() => favoriteLabelAlias.User.Id == userId && projectAlias.IsRemoved == false);

            if (projectType != null)
            {
                query.And(() => projectAlias.ProjectType == projectType.Value);
            }

            return query.OrderBy(() => favoriteLabelAlias.Name).Asc
                .OrderBy(() => favoriteItemAlias.Title).Asc
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }
        
        public virtual IList<FavoriteCategory> GetFavoriteLabeledCategories(int userId)
        {
            FavoriteCategory favoriteItemAlias = null;
            FavoriteLabel favoriteLabelAlias = null;

            return GetSession().QueryOver(() => favoriteItemAlias)
                .JoinAlias(() => favoriteItemAlias.FavoriteLabel, () => favoriteLabelAlias)
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .And(() => favoriteLabelAlias.User.Id == userId)
                .OrderBy(() => favoriteLabelAlias.Name).Asc
                .OrderBy(() => favoriteItemAlias.Title).Asc
                .List();
        }

        public virtual FavoriteLabel GetDefaultFavoriteLabel(int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;
            User userAlias = null;

            return GetSession().QueryOver(() => favoriteLabelAlias)
                .JoinAlias(() => favoriteLabelAlias.User, () => userAlias)
                .Where(() => favoriteLabelAlias.IsDefault && userAlias.Id == userId)
                .SingleOrDefault();
        }

        public virtual IList<FavoriteLabel> GetLatestFavoriteLabels(int latestLabelCount, int userId)
        {
            return GetSession().QueryOver<FavoriteLabel>()
                .Where(x => x.User.Id == userId)
                .OrderBy(x => x.LastUseTime).Desc
                .Take(latestLabelCount)
                .List();
        }
        
        public virtual IList<FavoriteLabel> GetAllFavoriteLabels(int userId)
        {
            return GetSession().QueryOver<FavoriteLabel>()
                .Where(x => x.User.Id == userId)
                .OrderBy(x => x.IsDefault).Desc
                .OrderBy(x => x.Name).Asc
                .List();
        }

        private string GetFavoriteTypeDiscriminatorValue(FavoriteTypeEnum favoriteType)
        {
            switch (favoriteType)
            {
                case FavoriteTypeEnum.Project:
                    return "Project";
                case FavoriteTypeEnum.Snapshot:
                    return "Snapshot";
                case FavoriteTypeEnum.Category:
                    return "Category";
                case FavoriteTypeEnum.Headword:
                    return "Headword";
                case FavoriteTypeEnum.Page:
                    return "Page";
                case FavoriteTypeEnum.Query:
                    return "Query";
                default:
                    return null;
            }
        }

        private IQueryOver<FavoriteBase, FavoriteBase> CreateGetFavoriteItemsQuery(long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;

            var query = GetSession().QueryOver<FavoriteBase>()
                .JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .Where(() => favoriteLabelAlias.User.Id == userId);

            if (labelId != null)
                query.And(x => x.FavoriteLabel.Id == labelId.Value);

            if (filterByType != null)
                query.And(x => x.FavoriteType == GetFavoriteTypeDiscriminatorValue(filterByType.Value));

            if (!string.IsNullOrWhiteSpace(filterByTitle))
            {
                filterByTitle = EscapeQuery(filterByTitle);
                query.AndRestrictionOn(x => x.Title).IsLike(filterByTitle, MatchMode.Anywhere);
            }

            return query;
        }

        public virtual ListWithTotalCountResult<FavoriteBase> GetFavoriteItems(long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, FavoriteSortEnumContract sort, int start, int count, int userId)
        {
            var query = CreateGetFavoriteItemsQuery(labelId, filterByType, filterByTitle, userId);

            switch (sort)
            {
                case FavoriteSortEnumContract.TitleAsc:
                    query = query.OrderBy(x => x.Title).Asc;
                    break;
                case FavoriteSortEnumContract.TitleDesc:
                    query = query.OrderBy(x => x.Title).Desc;
                    break;
                case FavoriteSortEnumContract.CreateTimeAsc:
                    query = query.OrderBy(x => x.CreateTime).Asc;
                    break;
                case FavoriteSortEnumContract.CreateTimeDesc:
                    query = query.OrderBy(x => x.CreateTime).Desc;
                    break;
                default:
                    query = query.OrderBy(x => x.Title).Asc;
                    break;
            }

            var resultList = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<FavoriteBase>
            {
                List = resultList.ToList(),
                Count = totalCount.Value,
            };
        }

        public virtual ListWithTotalCountResult<FavoriteQuery> GetFavoriteQueries(long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int start, int count, int userId)
        {
            FavoriteQuery favoriteQueryAlias = null;
            FavoriteLabel favoriteLabelAlias = null;
            BookType bookTypeAlias = null;

            var query = GetSession().QueryOver(() => favoriteQueryAlias)
                .JoinAlias(() => favoriteQueryAlias.FavoriteLabel, () => favoriteLabelAlias)
                .JoinAlias(() => favoriteQueryAlias.BookType, () => bookTypeAlias)
                .Where(() => bookTypeAlias.Type == bookTypeEnum && favoriteQueryAlias.QueryType == queryTypeEnum && favoriteLabelAlias.User.Id == userId)
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .Fetch(SelectMode.Fetch, x => x.BookType)
                .OrderBy(x => x.Title).Asc
                .OrderBy(() => favoriteLabelAlias.Name).Asc;

            if (labelId != null)
            {
                query.And(() => favoriteQueryAlias.FavoriteLabel.Id == labelId.Value);
            }

            if (!string.IsNullOrEmpty(filterByTitle))
            {
                filterByTitle = EscapeQuery(filterByTitle);
                query.AndRestrictionOn(() => favoriteQueryAlias.Title).IsLike(filterByTitle, MatchMode.Anywhere);
            }

            var list = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<FavoriteQuery>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }

        public virtual IList<FavoriteProject> GetFavoriteBooksWithLabel(BookTypeEnum bookType, ProjectTypeEnum? projectType, int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;
            Project projectAlias = null;
            Snapshot latestSnapshotAlias = null;
            BookType bookTypeAlias = null;
            
            var query = GetSession().QueryOver<FavoriteProject>()
                .JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .JoinAlias(x => x.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => latestSnapshotAlias)
                .JoinAlias(() => latestSnapshotAlias.BookTypes, () => bookTypeAlias)
                .Where(() => favoriteLabelAlias.User.Id == userId && bookTypeAlias.Type == bookType && projectAlias.IsRemoved == false);

            if (projectType != null)
            {
                query.And(() => projectAlias.ProjectType == projectType.Value);
            }

            return query.Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .OrderBy(x => x.Title).Asc
                .List();
        }

        public virtual IList<FavoriteCategory> GetFavoriteCategoriesWithLabel(int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;

            return GetSession().QueryOver<FavoriteCategory>()
                .JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .Where(() => favoriteLabelAlias.User.Id == userId )
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .OrderBy(x => x.Title).Asc
                .List();
        }

        public virtual FavoriteBase GetFavoriteItem(long favoriteId)
        {
            return GetSession().QueryOver<FavoriteBase>()
                .Where(x => x.Id == favoriteId)
                .Fetch(SelectMode.Fetch, x => x.FavoriteLabel)
                .SingleOrDefault();
        }
    }
}