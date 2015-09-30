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
    public class AuthorRepository : NHibernateTransactionalDao
    {
        public AuthorRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Author> GetAllAuthors()
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Author>().List<Author>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Author FindByName(string name)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Author>()
                    .Where(author => author.Name == name)
                    .SingleOrDefault<Author>();
            }
        }
    }
}