using System;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class BookRepository : NHibernateTransactionalDao
    {
        public BookRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Book> GetAllBooks()
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Book>().List<Book>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion GetLastVersionForBook(string bookGuid)
        {
            using (ISession session = GetSession())
            {
                var book = GetBookByGuid(bookGuid);
                BookVersion bookVersionAlias = null;

                var lastVersionSubquery = QueryOver.Of<BookVersion>()
                    .SelectList(l => l
                        .SelectGroup(item => item.Book.Id)
                        .SelectMax(item => item.CreateTime)
                    )
                    .Where(item => item.Book.Id == book.Id)
                    .Where(Restrictions.EqProperty(
                        Projections.Max<BookVersion>(item => item.CreateTime),
                        Projections.Property(() => bookVersionAlias.CreateTime)
                        ));


                var result = session.QueryOver<BookVersion>(() => bookVersionAlias)
                    .WithSubquery
                    .WhereExists(lastVersionSubquery)
                    .SingleOrDefault<BookVersion>();
                    

                return result;
            
            }
        }


        [Transaction(TransactionMode.Requires)]
        public virtual Book GetBookByGuid(string bookGuid)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Book>()
                    .Where(book => book.Guid == bookGuid)
                    .SingleOrDefault<Book>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateBook(string bookGuid, string name, string author)
        {
            var createTime = DateTime.UtcNow; //TODO pull it to method parameter, parse from XML
            var category = new Category(){Name = "cat"}; //TODO pull it to method parameter
            var versionId = "versionId"; //TODO pull it to method parameter, parse from XML
            var authors = new List<Author>();//TODO pull it to method parameter
            var description = "my new document"; //TODO pull it to method parameter, parse from XML
            using (ISession session = GetSession())
            {
                var bookId = (long)Create(new Book() { Guid = bookGuid, Category = category});
                var book = session.Load<Book>(bookId);
                Create(new BookVersion() { Book = book, Name = name, CreateTime = createTime , VersionId = versionId, Authors = authors, Description = description});
            }
          

        }
    }
}