using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class KeywordRepository : NHibernateTransactionalDao
    {
        public KeywordRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual LiteraryOriginal FindLiteraryOriginalByName(string name)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<LiteraryOriginal>()
                    .Where(author => author.Name == name)
                    .SingleOrDefault<LiteraryOriginal>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual LiteraryKind FindLiteraryKindByName(string name)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<LiteraryKind>()
                    .Where(author => author.Name == name)
                    .SingleOrDefault<LiteraryKind>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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