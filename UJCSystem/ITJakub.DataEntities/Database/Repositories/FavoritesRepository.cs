using System;
using System.Collections.Generic;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.SelectResults;
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
        public virtual void DeletePageBookmarkByPageXmlId(string bookId, string pageXmlId, string userName)
        {
            using (var session = GetSession())
            {
                PageBookmark pageBookmarkAlias = null;
                User userAlias = null;
                Book bookAlias = null;


                var bookmarks = session.QueryOver(()=>pageBookmarkAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.Book, () => bookAlias)     
                    .JoinQueryOver(()=>pageBookmarkAlias.User, ()=>userAlias)
                    .Where(() => pageBookmarkAlias.PageXmlId == pageXmlId && bookAlias.Guid == bookId && userAlias.UserName == userName)
                    .List<PageBookmark>();

                if (bookmarks == null)
                {
                    string message = string.Format("bookmark not found for bookId: '{0}' and page xmlId: '{1}' for user: '{2}'", bookId, pageXmlId, userName);
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
        public virtual IList<PageBookmark> GetAllPageBookmarksByBookId(string bookId, string userName)
        {
            using (var session = GetSession())
            {
                
                PageBookmark pageBookmarkAlias = null;
                User userAlias = null;
                Book bookAlias = null;

                return session.QueryOver(() => pageBookmarkAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.Book, () => bookAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.User, () => userAlias)
                    .Where(() => userAlias.UserName == userName && bookAlias.Guid == bookId)
                    .List<PageBookmark>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<PageBookmark> GetPageBookmarkByPageXmlId(string bookId, string pageXmlId, string userName)
        {
            using (var session = GetSession())
            {
                
                PageBookmark pageBookmarkAlias = null;
                User userAlias = null;
                Book bookAlias = null;

                return session.QueryOver(() => pageBookmarkAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.Book, () => bookAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.User, () => userAlias)
                    .Where(
                        () =>
                            userAlias.UserName == userName
                            && bookAlias.Guid == bookId
                            && pageBookmarkAlias.PageXmlId == pageXmlId)
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
                    .OrderBy(x => x.Name).Asc
                    .List();
            }
        }
    }
}