using System.Collections.Generic;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class AuthorRepository : NHibernateTransactionalDao
    {
        public AuthorRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<Author> GetAllAuthors()
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Author>().List<Author>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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