using System;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
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
                Book book = GetBookByGuid(bookGuid);
                BookVersion bookVersionAlias = null;

                QueryOver<BookVersion, BookVersion> lastVersionSubquery = QueryOver.Of<BookVersion>()
                    .SelectList(l => l
                        .SelectGroup(item => item.Book.Id)
                        .SelectMax(item => item.CreateTime)
                    )
                    .Where(item => item.Book.Id == book.Id)
                    .Where(Restrictions.EqProperty(
                        Projections.Max<BookVersion>(item => item.CreateTime),
                        Projections.Property(() => bookVersionAlias.CreateTime)
                        ));


                var result = session.QueryOver(() => bookVersionAlias)
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
        public virtual void CreateBook(string bookGuid, string title, string author)
        {
            DateTime createTime = DateTime.UtcNow;
            using (ISession session = GetSession())
            {
                var bookId = (long) Create(new Book {Guid = bookGuid});
                var book = session.Load<Book>(bookId);
                Create(new BookVersion
                {
                    Book = book,
                    Title = title,
                    CreateTime = createTime
                });
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void AssignAuthorsToBook(string bookGuid, string bookVersionGuid, IEnumerable<int> authorIds)
        {
            using (ISession session = GetSession())
            {
                BookVersion bookVersion = FindBookVersionByGuid(bookVersionGuid);
                foreach (int authorId in authorIds)
                {
                    var author = session.Load<Author>(authorId);
                    bookVersion.Authors.Add(author);
                }
                session.Update(bookVersion);
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

        [Transaction(TransactionMode.Requires)]
        public virtual string FindTransformationName(string documentId, string resultFormat) //TODO return transformation entity
        {
            return "pageToHtml.xsl"; //TODO resolve correct transformation and return its name
        }

        [Transaction(TransactionMode.Requires)]
        public BookType FindBookType(BookTypeEnum bookTypeEnum)
        {
            using (ISession session = GetSession())
            {
                return
                    session.QueryOver<BookType>()
                        .Where(bookType => bookType.Type == bookTypeEnum)
                        .SingleOrDefault<BookType>();
            }
        }
    }
}