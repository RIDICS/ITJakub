using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;

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
        public virtual int CreateAuthor(string name)
        {
            using (ISession session = GetSession())
            {
                return (int)Create(new Author(){ Name = name});
            }
        }
    }
}