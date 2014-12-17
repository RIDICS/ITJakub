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
            using (var session = GetSession())
            {
                return session.QueryOver<Book>().List<Book>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion GetLastVersionForBook(string bookGuid)
        {
            using (var session = GetSession())
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
            using (var session = GetSession())
            {
                return session.QueryOver<Book>()
                    .Where(book => book.Guid == bookGuid)
                    .SingleOrDefault<Book>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion FindBookVersionByGuid(string bookGuid, string bookVersionGuid)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<BookVersion>()
                        .Where(
                            bookVersion => bookVersion.VersionId == bookVersionGuid && bookVersion.Book.Guid == bookGuid)
                        .SingleOrDefault<BookVersion>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Transformation FindTransformation(BookVersion bookVersion, OutputFormatEnum outputFormat)
            //TODO return transformation entity
        {
            using (var session = GetSession())
            {
                var transformation = session.QueryOver<Transformation>()
                    .Where(transf => transf.OutputFormat == outputFormat && transf.BookVersions.Contains(bookVersion))
                    .SingleOrDefault<Transformation>();

                if (transformation == null)
                {
                    transformation = session.QueryOver<Transformation>()
                        .Where(
                            transf =>
                                transf.OutputFormat == outputFormat && transf.BookType == bookVersion.Book.BookType)
                        .SingleOrDefault<Transformation>();
                }

                return transformation;
            }
            //return "pageToHtml.xsl"; //TODO resolve correct transformation and return it
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookType FindBookType(BookTypeEnum bookTypeEnum)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<BookType>()
                        .Where(bookType => bookType.Type == bookTypeEnum)
                        .SingleOrDefault<BookType>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IEnumerable<BookVersion> GetAllVersionsByBookId(string bookId)
        {
            using (var session = GetSession())
            {
               return session.QueryOver<BookVersion>()
                        .JoinQueryOver(version => version.Book)
                        .Where(book => book.Guid == bookId).List<BookVersion>();
            }
        }
    }
}