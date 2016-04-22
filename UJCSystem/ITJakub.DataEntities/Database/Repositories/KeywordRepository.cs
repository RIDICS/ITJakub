
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;
using Remotion.Linq.Utilities;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class KeywordRepository : NHibernateTransactionalDao
    {
        public KeywordRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual LiteraryOriginal FindLiteraryOriginalByName(string name)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<LiteraryOriginal>()
                    .Where(author => author.Name == name)
                    .SingleOrDefault<LiteraryOriginal>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual LiteraryKind FindLiteraryKindByName(string name)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<LiteraryKind>()
                    .Where(author => author.Name == name)
                    .SingleOrDefault<LiteraryKind>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual LiteraryGenre FindLiteraryGenreByName(string name)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<LiteraryGenre>()
                    .Where(author => author.Name == name)
                    .SingleOrDefault<LiteraryGenre>();
            }
        }
    }
}