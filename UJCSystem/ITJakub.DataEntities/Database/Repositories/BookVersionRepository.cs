using System;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class BookVersionRepository : NHibernateTransactionalDao
    {
        public BookVersionRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }


        [Transaction(TransactionMode.Requires)]
        public virtual long Create(BookVersion bookVersion)
        {
            using (ISession session = GetSession())
            {
                bookVersion.CreateTime = DateTime.UtcNow;
                return (long)session.Save(bookVersion);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion FindBookVersionByGuid(string bookVersionGuid)
        {
            using (ISession session = GetSession())
            {
                return
                    session.QueryOver<BookVersion>()
                        .Where(bookVersion => bookVersion.VersionId == bookVersionGuid)
                        .SingleOrDefault<BookVersion>();
            }
        }
    }
}