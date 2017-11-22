using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.DataContracts.Types.Favorite;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class FavoritesRepository : NHibernateDao
    {
        public FavoritesRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
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
                .Fetch(x => x.FavoriteLabel).Eager
                .List();
        }

        //public virtual void DeleteHeadwordBookmark(string bookId, string entryXmlId, string userName)
        //{
        //    HeadwordBookmark headwordBookmark = null;
        //    User userAlias = null;
        //    Book bookAlias = null;


        //    var bookmarks = GetSession().QueryOver(() => headwordBookmark)
        //        .JoinQueryOver(() => headwordBookmark.Book, () => bookAlias)
        //        .JoinQueryOver(() => headwordBookmark.User, () => userAlias)
        //        .Where(() => headwordBookmark.XmlEntryId == entryXmlId && bookAlias.Guid == bookId && userAlias.UserName == userName)
        //        .List<HeadwordBookmark>();

        //    if (bookmarks == null)
        //    {
        //        string message = string.Format("bookmark not found for bookId: '{0}' and headword xmlId: '{1}' for user: '{2}'", bookId, entryXmlId, userName);
        //        if (m_log.IsErrorEnabled)
        //            m_log.Error(message);
        //        throw new ArgumentException(message);
        //    }

        //    foreach (var bookmark in bookmarks)
        //    {
        //        Delete(bookmark);
        //    }
        //}

        //public virtual IList<HeadwordBookmarkResult> GetAllHeadwordBookmarks(string userName)
        //{
        //    HeadwordBookmarkResult resultAlias = null;
        //    HeadwordBookmark headwordBookmarkAlias = null;
        //    User userAlias = null;
        //    Book bookAlias = null;
        //    BookVersion bookVersionAlias = null;
        //    BookHeadword bookHeadwordAlias = null;

        //    return GetSession().QueryOver(() => headwordBookmarkAlias)
        //        .JoinQueryOver(() => headwordBookmarkAlias.Book, () => bookAlias)
        //        .JoinQueryOver(() => headwordBookmarkAlias.User, () => userAlias)
        //        .JoinQueryOver(() => bookAlias.LastVersion, () => bookVersionAlias)
        //        .JoinQueryOver(() => bookVersionAlias.BookHeadwords, () => bookHeadwordAlias)
        //        .Select(Projections.Distinct(Projections.ProjectionList()
        //            .Add(Projections.Property(() => headwordBookmarkAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))
        //            .Add(Projections.Property(() => bookAlias.Guid).WithAlias(() => resultAlias.BookGuid))
        //            .Add(Projections.Property(() => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword))
        //            ))
        //        .Where(() => userAlias.UserName == userName)
        //        .And(() => headwordBookmarkAlias.XmlEntryId == bookHeadwordAlias.XmlEntryId)
        //        .TransformUsing(Transformers.AliasToBean<HeadwordBookmarkResult>())
        //        .List<HeadwordBookmarkResult>();
        //}

        public virtual IList<FavoriteLabel> GetFavoriteLabelsById(IList<long> labelIds)
        {
            return GetSession().QueryOver<FavoriteLabel>()
                .WhereRestrictionOn(x => x.Id).IsInG(labelIds)
                .List();
        }

        public virtual IList<FavoriteProject> GetFavoriteLabeledBooks(IList<long> projectIds, int userId)
        {
            FavoriteProject favoriteItemAlias = null;
            FavoriteLabel favoriteLabelAlias = null;

            return GetSession().QueryOver(() => favoriteItemAlias)
                .JoinAlias(() => favoriteItemAlias.FavoriteLabel, () => favoriteLabelAlias)
                .Fetch(x => x.FavoriteLabel).Eager
                .WhereRestrictionOn(() => favoriteItemAlias.Project.Id).IsInG(projectIds)
                .And(() => favoriteLabelAlias.User.Id == userId)
                .OrderBy(() => favoriteLabelAlias.Name).Asc
                .OrderBy(() => favoriteItemAlias.Title).Asc
                .List();
        }
        
        public virtual IList<FavoriteCategory> GetFavoriteLabeledCategories(IList<int> categoryIds, int userId)
        {
            FavoriteCategory favoriteItemAlias = null;
            FavoriteLabel favoriteLabelAlias = null;

            return GetSession().QueryOver(() => favoriteItemAlias)
                .JoinAlias(() => favoriteItemAlias.FavoriteLabel, () => favoriteLabelAlias)
                .Fetch(x => x.FavoriteLabel).Eager
                .WhereRestrictionOn(() => favoriteItemAlias.Category.Id).IsInG(categoryIds)
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

        private IQueryOver<FavoriteBase, FavoriteBase> CreateGetFavoriteItemsQuery(ISession session, long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, int userId)
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
                query.AndRestrictionOn(x => x.Title).IsLike(filterByTitle, MatchMode.Anywhere);

            return query;
        }

        public virtual IList<FavoriteBase> GetFavoriteItems(long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, FavoriteSortEnumContract sort, int start, int count, int userId)
        {
            var query = CreateGetFavoriteItemsQuery(GetSession(), labelId, filterByType, filterByTitle, userId);

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

            return query.Skip(start)
                .Take(count)
                .List();
        }

        public virtual int GetFavoriteItemsCount(long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, int userId)
        {
            var query = CreateGetFavoriteItemsQuery(GetSession(), labelId, filterByType, filterByTitle, userId);
            return query.RowCount();
        }

        private IQueryOver<FavoriteQuery, FavoriteQuery> CreateGetFavoriteQueriesQuery(ISession session, long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int userId)
        {
            FavoriteQuery favoriteQueryAlias = null;
            FavoriteLabel favoriteLabelAlias = null;
            BookType bookTypeAlias = null;

            var query = GetSession().QueryOver(() => favoriteQueryAlias)
                .JoinAlias(() => favoriteQueryAlias.FavoriteLabel, () => favoriteLabelAlias)
                .JoinAlias(() => favoriteQueryAlias.BookType, () => bookTypeAlias)
                .Where(() => bookTypeAlias.Type == bookTypeEnum && favoriteQueryAlias.QueryType == queryTypeEnum && favoriteLabelAlias.User.Id == userId);

            if (labelId != null)
            {
                query.And(() => favoriteQueryAlias.FavoriteLabel.Id == labelId.Value);
            }
            if (!string.IsNullOrEmpty(filterByTitle))
            {
                query.AndRestrictionOn(() => favoriteQueryAlias.Title).IsLike(filterByTitle, MatchMode.Anywhere);
            }

            return query;
        }

        public virtual IList<FavoriteQuery> GetFavoriteQueries(long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int start, int count, int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;

            var query = CreateGetFavoriteQueriesQuery(GetSession(), labelId, bookTypeEnum, queryTypeEnum, filterByTitle, userId);

            return query.JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .Fetch(x => x.FavoriteLabel).Eager
                .Fetch(x => x.BookType).Eager
                .OrderBy(x => x.Title).Asc
                .OrderBy(() => favoriteLabelAlias.Name).Asc
                .Skip(start)
                .Take(count)
                .List();
        }
        
        public virtual int GetFavoriteQueriesCount(long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int userId)
        {
            var query = CreateGetFavoriteQueriesQuery(GetSession(), labelId, bookTypeEnum, queryTypeEnum, filterByTitle, userId);
            return query.RowCount();
        }

        public virtual IList<FavoriteProject> GetFavoriteBooksWithLabel(BookTypeEnum bookType, int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;
            Project projectAlias = null;
            Snapshot latestSnapshotAlias = null;
            BookType bookTypeAlias = null;
            
            return GetSession().QueryOver<FavoriteProject>()
                .JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .JoinAlias(x => x.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => latestSnapshotAlias)
                .JoinAlias(() => latestSnapshotAlias.BookTypes, () => bookTypeAlias)
                .Where(() => favoriteLabelAlias.User.Id == userId && bookTypeAlias.Type == bookType)
                .Fetch(x => x.FavoriteLabel).Eager
                .OrderBy(x => x.Title).Asc
                .List();
        }

        public virtual IList<FavoriteCategory> GetFavoriteCategoriesWithLabel(BookTypeEnum bookType, int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;
            Project projectAlias = null;
            Snapshot latestSnapshotAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            return GetSession().QueryOver<FavoriteCategory>()
                .JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                .JoinAlias(x => x.Category, () => categoryAlias)
                .JoinAlias(() => categoryAlias.Projects, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => latestSnapshotAlias)
                .JoinAlias(() => latestSnapshotAlias.BookTypes, () => bookTypeAlias)
                .Where(() => favoriteLabelAlias.User.Id == userId && bookTypeAlias.Type == bookType)
                .Fetch(x => x.FavoriteLabel).Eager
                .OrderBy(x => x.Title).Asc
                .List();
        }

        public virtual FavoriteBase GetFavoriteItem(long favoriteId)
        {
            return GetSession().QueryOver<FavoriteBase>()
                .Where(x => x.Id == favoriteId)
                .Fetch(x => x.FavoriteLabel).Eager
                .SingleOrDefault();
        }
    }
}