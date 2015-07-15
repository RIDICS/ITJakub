using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

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
                var book = FindBookByGuid(bookGuid);

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
        public virtual BookVersion GetLastVersionForBookWithPages(string bookGuid)
        {
            using (var session = GetSession())
            {
                var book = FindBookByGuid(bookGuid);

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
                    .Fetch(x => x.BookPages).Eager
                    .SingleOrDefault<BookVersion>();


                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Book FindBookByGuid(string bookGuid)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<Book>()
                    .Where(book => book.Guid == bookGuid)
                    .SingleOrDefault<Book>();
                return result;
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
        public virtual Transformation FindTransformation(BookVersion bookVersion, OutputFormat outputFormat, BookTypeEnum requestedBookType)
        {

            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;

            using (var session = GetSession())
            {
                var transformation = session.QueryOver<Transformation>()
                    .JoinAlias( t => t.BookVersions, () => bookVersionAlias)
                    .JoinAlias( t => t.BookType, () => bookTypeAlias)
                    .Where( t => t.OutputFormat == outputFormat && bookVersionAlias.Id == bookVersion.Id && bookTypeAlias.Type == requestedBookType)
                    .SingleOrDefault<Transformation>();

                //TODO could be looked up for specific book transformation (shared between version)

                if (transformation == null)
                {
                    transformation = session.QueryOver<Transformation>()
                        .Where(
                            t =>
                                t.OutputFormat == outputFormat && t.BookType.Id == bookVersion.DefaultBookType.Id && t.IsDefaultForBookType)
                        .SingleOrDefault<Transformation>();
                }

                return transformation;
            }
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

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> SearchByTitle(string text)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersion = null;

                // subquery to be later used for EXISTS
                var maxSubquery = QueryOver.Of<BookVersion>()
                    .SelectList(l => l
                        .SelectGroup(item => item.Book.Id)
                        .SelectMax(item => item.CreateTime)
                    )
                    // WHERE Clause
                    .Where(x => x.Book.Id == bookVersion.Book.Id)
                    // HAVING Clause
                    .Where(Restrictions.EqProperty(
                        Projections.Max<BookVersion>(item => item.CreateTime),
                        Projections.Property(() => bookVersion.CreateTime)
                        ));

                // final query without any transformations/projections... but filtered
                var result = session.QueryOver(() => bookVersion)
                    .WhereRestrictionOn(x => x.Title).IsLike(string.Format("%{0}%", text))
                    .WithSubquery
                    .WhereExists(maxSubquery)
                    .List<BookVersion>();

                //var pagesCount = result.First().BookPages.Count;
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> SearchByTitleAndBookType(string text, BookTypeEnum bookType)
        {
            Book bookAlias = null;
            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            using (var session = GetSession())
            {
                var bookVersions =
                    session.QueryOver(() => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Categories, () => categoryAlias, JoinType.InnerJoin)
                        .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias, JoinType.InnerJoin)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias, JoinType.InnerJoin)
                        .Where(() => bookTypeAlias.Type == bookType && bookVersionAlias.Id == bookAlias.LastVersion.Id && bookVersionAlias.Title.IsLike(string.Format("%{0}%", text)))
                        .List<BookVersion>();
                return bookVersions;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> FindBooksLastVersionsByBookType(BookTypeEnum bookType)
        {
            Book bookAlias = null;
            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            using (var session = GetSession())
            {
                var bookVersions =
                    session.QueryOver(() => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Categories, () => categoryAlias)
                        .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(() => bookTypeAlias.Type == bookType && bookVersionAlias.Id == bookAlias.LastVersion.Id)
                        .OrderBy(() => bookVersionAlias.Title).Asc
                        .TransformUsing(Transformers.DistinctRootEntity)
                        .List<BookVersion>();

                return bookVersions;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetLastAuthors(int recordCount)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Author>()
                    .Select(x => x.Name)
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetLastAuthorsByBookType(int recordCount, BookTypeEnum bookType)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                Author authorAlias = null;
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => bookVersionAlias.Authors, () => authorAlias)
                    .JoinQueryOver(x => bookVersionAlias.Categories, () => categoryAlias)
                    .JoinQueryOver(x => categoryAlias.BookType, () => bookTypeAlias)
                    .Select(Projections.Distinct(Projections.Property(() => authorAlias.Name)))
                    .Where(x => bookTypeAlias.Type == bookType)
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetTypeaheadAuthors(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                Author authorAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Authors, () => authorAlias)
                    .Select(Projections.Distinct(Projections.Property(() => authorAlias.Name)))
                    .WhereRestrictionOn(() => authorAlias.Name).IsInsensitiveLike(query)
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnum bookType, int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                Author authorAlias = null;
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Authors, () => authorAlias)
                    .JoinQueryOver(x => bookVersionAlias.Categories, () => categoryAlias)
                    .JoinQueryOver(x => categoryAlias.BookType, () => bookTypeAlias)
                    .Select(Projections.Distinct(Projections.Property(() => authorAlias.Name)))
                    .Where(x => bookTypeAlias.Type == bookType)
                    .AndRestrictionOn(() => authorAlias.Name).IsInsensitiveLike(query)
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetLastTitles(int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .Select(x => bookVersionAlias.Title)
                    .OrderBy(() => bookVersionAlias.CreateTime).Desc
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetLastTitlesByBookType(int recordCount, BookTypeEnum bookType)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Categories, () => categoryAlias)
                    .JoinQueryOver(x => categoryAlias.BookType, () => bookTypeAlias)
                    .Select(x => bookVersionAlias.Title)
                    .Where(x => bookTypeAlias.Type == bookType)
                    .OrderBy(() => bookVersionAlias.CreateTime).Desc
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetTypeaheadTitles(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .Select(x => bookVersionAlias.Title)
                    .WhereRestrictionOn(() => bookVersionAlias.Title).IsInsensitiveLike(query)
                    .Take(recordCount)
                    .List<string>();
            }
        }
        
        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnum bookType, int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Categories, () => categoryAlias)
                    .JoinQueryOver(x => categoryAlias.BookType, () => bookTypeAlias)
                    .Select(x => bookVersionAlias.Title)
                    .Where(x => bookTypeAlias.Type == bookType)
                    .AndRestrictionOn(() => bookVersionAlias.Title).IsInsensitiveLike(query)
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetLastTypeaheadHeadwords(int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookHeadwordAlias.Headword)))
                    .Where(x => x.Visibility == VisibilityEnum.Public)
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetTypeaheadHeadwords(string query, int recordCount)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;

                return session.QueryOver<Book>()
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookHeadwordAlias.Headword)))
                    .Where(x => x.Visibility == VisibilityEnum.Public)
                    .AndRestrictionOn(x => x.Headword).IsInsensitiveLike(query)
                    .Take(recordCount)
                    .List<string>();
            }
        }
    }
}