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
        public virtual int CreateAuthor(IEnumerable<AuthorInfo> authorInfos)
        {
            using (ISession session = GetSession())
            {
                var authorId = (int)Create(new Author());
                var author = session.Load<Author>(authorId);
                foreach (var authorInfo in authorInfos)
                {
                    authorInfo.Author = author;
                    Create(authorInfo);
                }
                return authorId;
            }
        }
    }
}