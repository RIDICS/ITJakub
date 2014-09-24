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
        public virtual void CreateAuthor(IEnumerable<AuthorInfo> authorInfoDtos) //TODO make dto
        {
            using (ISession session = GetSession())
            {
                var authorId = (long)Create(new Author());
                var author = session.Load<Author>(authorId);
                foreach (var authorInfoDto in authorInfoDtos)
                {
                    var authorInfo = new AuthorInfo() {Author = author, Text = authorInfoDto.Text, TextType = authorInfoDto.TextType};
                    Create(authorInfo);
                }
            }
        }
    }
}