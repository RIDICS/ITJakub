using System;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class FavoritesRepository:NHibernateTransactionalDao
    {
        public FavoritesRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeletePageBookmarkByPageXmlId(string bookId, string pageXmlId, string userName)
        {
            using (var session = GetSession())
            {

                 PageBookmark pageBookmarkAlias = null;
                User userAlias = null;
                Book bookAlias = null;


                var bookmarks = session.QueryOver<PageBookmark>(()=>pageBookmarkAlias)
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

        [Transaction(TransactionMode.Requires)]
        public virtual IList<PageBookmark> GetAllPageBookmarksByBookId(string bookId, string userName)
        {
            using (var session = GetSession())
            {
                
                 PageBookmark pageBookmarkAlias = null;
                User userAlias = null;
                Book bookAlias = null;

                return session.QueryOver<PageBookmark>(() => pageBookmarkAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.Book, () => bookAlias)
                    .JoinQueryOver(() => pageBookmarkAlias.User, () => userAlias)
                    .Where(() => userAlias.UserName == userName && bookAlias.Guid == bookId)
                    .List<PageBookmark>();
            }
        }
    }
}