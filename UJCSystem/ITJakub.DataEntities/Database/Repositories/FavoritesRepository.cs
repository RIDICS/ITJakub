using System;
using System.Collections.Generic;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.Shared.Contracts.Favorites;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class FavoritesRepository:NHibernateTransactionalDao
    {
        public FavoritesRepository(ISessionManager sessManager) : base(sessManager)
        {
        }
        
        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<PageBookmark> GetAllPageBookmarksByBookId(string bookXmlId, long userId)
        {
            using (var session = GetSession())
            {
                PageBookmark pageBookmarkAlias = null;
                Book bookAlias = null;

                return session.QueryOver(() => pageBookmarkAlias)
                    .JoinAlias(() => pageBookmarkAlias.Book, () => bookAlias)
                    .Where(() => bookAlias.Guid == bookXmlId && pageBookmarkAlias.User.Id == userId)
                    .Fetch(x => x.FavoriteLabel).Eager
                    .List<PageBookmark>();
            }
        }
        
        [Transaction(TransactionScopeOption.Required)]
        public virtual void DeleteHeadwordBookmark(string bookId, string entryXmlId, string userName)
        {
            using (var session = GetSession())
            {
                HeadwordBookmark headwordBookmark = null;
                User userAlias = null;
                Book bookAlias = null;


                var bookmarks = session.QueryOver(() => headwordBookmark)
                    .JoinQueryOver(() => headwordBookmark.Book, () => bookAlias)
                    .JoinQueryOver(() => headwordBookmark.User, () => userAlias)
                    .Where(() => headwordBookmark.XmlEntryId == entryXmlId && bookAlias.Guid == bookId && userAlias.UserName == userName)
                    .List<HeadwordBookmark>();

                if (bookmarks == null)
                {
                    string message = string.Format("bookmark not found for bookId: '{0}' and headword xmlId: '{1}' for user: '{2}'", bookId, entryXmlId, userName);
                    if (m_log.IsErrorEnabled)
                        m_log.Error(message);
                    throw new ArgumentException(message);
                }

                foreach (var bookmark in bookmarks)
                {
                    Delete(bookmark);
                }                
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<HeadwordBookmarkResult> GetAllHeadwordBookmarks(string userName)
        {
            using (var session = GetSession())
            {
                HeadwordBookmarkResult resultAlias = null;
                HeadwordBookmark headwordBookmarkAlias = null;
                User userAlias = null;
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;

                return session.QueryOver(() => headwordBookmarkAlias)
                    .JoinQueryOver(() => headwordBookmarkAlias.Book, () => bookAlias)
                    .JoinQueryOver(() => headwordBookmarkAlias.User, () => userAlias)
                    .JoinQueryOver(() => bookAlias.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(() => bookVersionAlias.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.ProjectionList()
                        .Add(Projections.Property(() => headwordBookmarkAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))
                        .Add(Projections.Property(() => bookAlias.Guid).WithAlias(() => resultAlias.BookGuid))
                        .Add(Projections.Property(() => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword))
                        ))
                    .Where(() => userAlias.UserName == userName)
                    .And(() => headwordBookmarkAlias.XmlEntryId == bookHeadwordAlias.XmlEntryId)
                    .TransformUsing(Transformers.AliasToBean<HeadwordBookmarkResult>())
                    .List<HeadwordBookmarkResult>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteLabel> GetFavoriteLabelsById(IList<long> labelIds)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<FavoriteLabel>()
                    .WhereRestrictionOn(x => x.Id).IsInG(labelIds)
                    .List();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteBook> GetFavoriteLabeledBooks(IList<long> bookIds, int userId)
        {
            FavoriteBook favoriteItemAlias = null;
            FavoriteLabel favoriteLabelAlias = null;

            using (var session = GetSession())
            {
                return session.QueryOver(() => favoriteItemAlias)
                    .JoinAlias(() => favoriteItemAlias.FavoriteLabel, () => favoriteLabelAlias)
                    .Fetch(x => x.FavoriteLabel).Eager
                    .WhereRestrictionOn(() => favoriteItemAlias.Book.Id).IsInG(bookIds)
                    .And(() => favoriteLabelAlias.User.Id == userId)
                    .OrderBy(() => favoriteLabelAlias.Name).Asc
                    .OrderBy(() => favoriteItemAlias.Title).Asc
                    .List();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteCategory> GetFavoriteLabeledCategories(IList<int> categoryIds, int userId)
        {
            FavoriteCategory favoriteItemAlias = null;
            FavoriteLabel favoriteLabelAlias = null;

            using (var session = GetSession())
            {
                return session.QueryOver(() => favoriteItemAlias)
                    .JoinAlias(() => favoriteItemAlias.FavoriteLabel, () => favoriteLabelAlias)
                    .Fetch(x => x.FavoriteLabel).Eager
                    .WhereRestrictionOn(() => favoriteItemAlias.Category.Id).IsInG(categoryIds)
                    .And(() => favoriteLabelAlias.User.Id == userId)
                    .OrderBy(() => favoriteLabelAlias.Name).Asc
                    .OrderBy(() => favoriteItemAlias.Title).Asc
                    .List();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual FavoriteLabel GetDefaultFavoriteLabel(int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;
            User userAlias = null;

            using (var session = GetSession())
            {
                return session.QueryOver(() => favoriteLabelAlias)
                    .JoinAlias(() => favoriteLabelAlias.User, () => userAlias)
                    .Where(() => favoriteLabelAlias.IsDefault && userAlias.Id == userId)
                    .SingleOrDefault();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteLabel> GetLatestFavoriteLabels(int latestLabelCount, int userId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<FavoriteLabel>()
                    .Where(x => x.User.Id == userId)
                    .OrderBy(x => x.LastUseTime).Desc
                    .Take(latestLabelCount)
                    .List();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteLabel> GetAllFavoriteLabels(int userId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<FavoriteLabel>()
                    .Where(x => x.User.Id == userId)
                    .OrderBy(x => x.IsDefault).Desc
                    .OrderBy(x => x.Name).Asc
                    .List();
            }
        }

        private string GetFavoriteTypeDiscriminatorValue(FavoriteTypeEnum favoriteType)
        {
            switch (favoriteType)
            {
                case FavoriteTypeEnum.Book:
                    return "Book";
                case FavoriteTypeEnum.BookVersion:
                    return "BookVersion";
                case FavoriteTypeEnum.Category:
                    return "Category";
                case FavoriteTypeEnum.HeadwordBookmark:
                    return "HeadwordBookmark";
                case FavoriteTypeEnum.PageBookmark:
                    return "PageBookmark";
                case FavoriteTypeEnum.Query:
                    return "Query";
                default:
                    return null;
            }
        }

        private IQueryOver<FavoriteBase, FavoriteBase> CreateFavoriteItemsQuery(ISession session, long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, int userId)
        {
            var query = session.QueryOver<FavoriteBase>()
                .Where(x => x.User.Id == userId);

            if (labelId != null)
                query.And(x => x.FavoriteLabel.Id == labelId.Value);

            if (filterByType != null)
                query.And(x => x.FavoriteType == GetFavoriteTypeDiscriminatorValue(filterByType.Value));

            if (!string.IsNullOrWhiteSpace(filterByTitle))
                query.AndRestrictionOn(x => x.Title).IsLike(filterByTitle, MatchMode.Anywhere);

            return query;
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteBase> GetFavoriteItems(long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, FavoriteSortContract sort, int start, int count, int userId)
        {
            using (var session = GetSession())
            {
                var query = CreateFavoriteItemsQuery(session, labelId, filterByType, filterByTitle, userId);

                switch (sort)
                {
                    case FavoriteSortContract.TitleAsc:
                        query = query.OrderBy(x => x.Title).Asc;
                        break;
                    case FavoriteSortContract.TitleDesc:
                        query = query.OrderBy(x => x.Title).Desc;
                        break;
                    case FavoriteSortContract.CreateTimeAsc:
                        query = query.OrderBy(x => x.CreateTime).Asc;
                        break;
                    case FavoriteSortContract.CreateTimeDesc:
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
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual int GetFavoriteItemsCount(long? labelId, FavoriteTypeEnum? filterByType, string filterByTitle, int userId)
        {
            using (var session = GetSession())
            {
                var query = CreateFavoriteItemsQuery(session, labelId, filterByType, filterByTitle, userId);
                return query.RowCount();
            }
        }
        
        private IQueryOver<FavoriteQuery, FavoriteQuery> CreateFavoriteQueriesQuery(ISession session, long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int userId)
        {
            FavoriteQuery favoriteQueryAlias = null;
            BookType bookTypeAlias = null;

            var query = session.QueryOver(() => favoriteQueryAlias)
                .JoinAlias(() => favoriteQueryAlias.BookType, () => bookTypeAlias)
                .Where(() => bookTypeAlias.Type == bookTypeEnum && favoriteQueryAlias.QueryType == queryTypeEnum && favoriteQueryAlias.User.Id == userId);

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

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteQuery> GetFavoriteQueries(long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int start, int count, int userId)
        {
            FavoriteLabel favoriteLabelAlias = null;

            using (var session = GetSession())
            {
                var query = CreateFavoriteQueriesQuery(session, labelId, bookTypeEnum, queryTypeEnum, filterByTitle, userId);

                return query.JoinAlias(x => x.FavoriteLabel, () => favoriteLabelAlias)
                    .Fetch(x => x.FavoriteLabel).Eager
                    .Fetch(x => x.BookType).Eager
                    .OrderBy(x => x.Title).Asc
                    .OrderBy(() => favoriteLabelAlias.Name).Asc
                    .Skip(start)
                    .Take(count)
                    .List();
            }
        }
        
        [Transaction(TransactionScopeOption.Required)]
        public virtual int GetFavoriteQueriesCount(long? labelId, BookTypeEnum bookTypeEnum, QueryTypeEnum queryTypeEnum, string filterByTitle, int userId)
        {
            using (var session = GetSession())
            {
                var query = CreateFavoriteQueriesQuery(session, labelId, bookTypeEnum, queryTypeEnum, filterByTitle, userId);
                return query.RowCount();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteBook> GetFavoriteBooksWithLabel(BookTypeEnum bookType, int userId)
        {
            Book bookAlias = null;
            BookVersion bookVersionAlias = null;
            Category categoryAlias = null;
            BookType bookTypeAlias = null;

            using (var session = GetSession())
            {
                return session.QueryOver<FavoriteBook>()
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .JoinAlias(() => bookAlias.LastVersion, () => bookVersionAlias)
                    .JoinAlias(() => bookVersionAlias.Categories, () => categoryAlias)
                    .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias)
                    .Where(x => x.User.Id == userId && bookTypeAlias.Type == bookType)
                    .Fetch(x => x.FavoriteLabel).Eager
                    .OrderBy(x => x.Title).Asc
                    .List();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<FavoriteCategory> GetFavoriteCategoriesWithLabel(BookTypeEnum bookType, int userId)
        {
            Category categoryAlias = null;
            BookType bookTypeAlias = null;

            using (var session = GetSession())
            {
                return session.QueryOver<FavoriteCategory>()
                    .JoinAlias(x => x.Category, () => categoryAlias)
                    .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias)
                    .Where(x => x.User.Id == userId && bookTypeAlias.Type == bookType)
                    .Fetch(x => x.FavoriteLabel).Eager
                    .OrderBy(x => x.Title).Asc
                    .List();
            }
        }
    }
}