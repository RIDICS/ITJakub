using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using ResponsibleType = ITJakub.DataEntities.Database.Entities.ResponsibleType;

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
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var result = session.QueryOver(() => bookVersionAlias)
                    .JoinQueryOver(x => x.Book, () => bookAlias)
                    .Where(x => x.Guid == bookGuid)
                    .And(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .SingleOrDefault();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion GetLastVersionForBookWithType(string bookGuid)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var result = session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.Guid == bookGuid)
                    .And(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .Fetch(x => x.DefaultBookType).Eager
                    .SingleOrDefault();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion GetLastVersionForBookWithPages(string bookGuid)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var result = session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.Guid == bookGuid)
                    .And(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .Fetch(x => x.BookPages).Eager
                    .SingleOrDefault();

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
                
                if (transformation == null)
                {
                    transformation = session.QueryOver<Transformation>()
                        .JoinAlias(x => x.BookType, () => bookTypeAlias)
                        .Where(x => x.OutputFormat == outputFormat && x.IsDefaultForBookType && bookTypeAlias.Type == requestedBookType )
                        .SingleOrDefault<Transformation>();
                }
                
                return transformation;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Transformation FindDefaultTransformation(BookVersion bookVersion, OutputFormat outputFormat)
        {
            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;

            using (var session = GetSession())
            {
                var transformation = session.QueryOver<Transformation>()
                    .JoinAlias(t => t.BookVersions, () => bookVersionAlias)
                    .JoinAlias(t => t.BookType, () => bookTypeAlias)
                    .Where(t => t.OutputFormat == outputFormat && bookVersionAlias.Id == bookVersion.Id && bookTypeAlias.Id == bookVersion.DefaultBookType.Id)
                    .SingleOrDefault<Transformation>();

                if (transformation == null)
                {
                    transformation = session.QueryOver<Transformation>()
                        .Where(
                            t =>
                                t.OutputFormat == outputFormat && t.IsDefaultForBookType && bookTypeAlias.Id == bookVersion.DefaultBookType.Id)
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
        public virtual IEnumerable<BookVersion> GetAllVersionsByBookXmlId(string bookXmlId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<BookVersion>()
                    .JoinQueryOver(version => version.Book)
                    .Where(book => book.Guid == bookXmlId).List<BookVersion>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> SearchByTitle(string text)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                Responsible responsibleAlias = null;
                ResponsibleType responsibleTypeAlias = null;

                var futureResult = session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => x.Title).IsLike(text, MatchMode.Anywhere)
                    .Fetch(x => x.Book).Eager
                    .Fetch(x => x.Publisher).Eager
                    .Fetch(x => x.DefaultBookType).Eager
                    .Fetch(x => x.ManuscriptDescriptions).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => x.Title).IsLike(text, MatchMode.Anywhere)
                    .Fetch(x => x.Keywords).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => x.Title).IsLike(text, MatchMode.Anywhere)
                    .Fetch(x => x.Authors).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => x.Title).IsLike(text, MatchMode.Anywhere)
                    .Left.JoinAlias(() => bookVersionAlias.Responsibles, () => responsibleAlias)
                    .Left.JoinAlias(() => responsibleAlias.ResponsibleType, () => responsibleTypeAlias)
                    .Future<BookVersion>();

                var result = futureResult.ToList();
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
                        .Where(() => bookTypeAlias.Type == bookType && bookVersionAlias.Id == bookAlias.LastVersion.Id)
                        .AndRestrictionOn(() => bookVersionAlias.Title).IsLike(text, MatchMode.Anywhere)
                        .Fetch(x => x.Authors).Eager
                        .TransformUsing(Transformers.DistinctRootEntity)
                        .List<BookVersion>();
                return bookVersions;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> SearchByAuthorAndBookType(string query, BookTypeEnum bookType)
        {
            Book bookAlias = null;
            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;
            Author authorAlias = null;

            using (var session = GetSession())
            {
                var bookIds = QueryOver.Of(() => bookVersionAlias)
                    .JoinAlias(() => bookVersionAlias.Categories, () => categoryAlias, JoinType.InnerJoin)
                    .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias, JoinType.InnerJoin)
                    .JoinAlias(() => bookVersionAlias.Book, () => bookAlias, JoinType.InnerJoin)
                    .JoinAlias(() => bookVersionAlias.Authors, () => authorAlias, JoinType.InnerJoin)
                    .Select(x => x.Id)
                    .Where(() => bookTypeAlias.Type == bookType && bookVersionAlias.Id == bookAlias.LastVersion.Id)
                    .AndRestrictionOn(() => authorAlias.Name).IsLike(query, MatchMode.Anywhere);

                var bookVersions = session.QueryOver(() => bookVersionAlias)
                    .Fetch(x => x.Book).Eager
                    .Fetch(x => x.Authors).Eager
                    .WithSubquery
                    .WhereProperty(x => x.Id)
                    .In(bookIds)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();

                return bookVersions;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> FindBookVersionsByTypeWithCategories(BookTypeEnum bookType)
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
                        .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookVersionAlias.Id == bookAlias.LastVersion.Id)
                    .Fetch(x => x.Categories).Eager
                    .Future<BookVersion>();

                return bookVersions.ToList();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> FindBookVersionsByTypeWithAuthors(BookTypeEnum bookType)
        {
            Book bookAlias = null;
            BookVersion bookVersionAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            using (var session = GetSession())
            {
                var result = session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(() => bookVersionAlias.Categories, () => categoryAlias)
                    .JoinAlias(() => categoryAlias.BookType, () => bookTypeAlias)
                    .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                    .Where(() => bookTypeAlias.Type == bookType && bookVersionAlias.Id == bookAlias.LastVersion.Id)
                    .Fetch(x => x.Authors).Eager
                    .OrderBy(() => bookVersionAlias.Title).Asc
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();

                return result;
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
        public virtual IList<string> GetLastTitlesByBookType(int recordCount, BookTypeEnum bookType, IList<long> bookIdList)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                var query = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Categories, () => categoryAlias)
                    .JoinQueryOver(x => categoryAlias.BookType, () => bookTypeAlias)
                    .Select(x => bookVersionAlias.Title)
                    .Where(x => bookTypeAlias.Type == bookType);

                if (bookIdList != null)
                    query.AndRestrictionOn(() => bookAlias.Id).IsInG(bookIdList);
                    
                return query
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
        public virtual IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnum bookType, IList<long> bookIdList, int recordCount)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                Category categoryAlias = null;
                BookType bookTypeAlias = null;

                var dbQuery = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Categories, () => categoryAlias)
                    .JoinQueryOver(x => categoryAlias.BookType, () => bookTypeAlias)
                    .Select(x => bookVersionAlias.Title)
                    .Where(x => bookTypeAlias.Type == bookType)
                    .AndRestrictionOn(() => bookVersionAlias.Title).IsInsensitiveLike(query);

                if (bookIdList != null)
                    dbQuery.AndRestrictionOn(() => bookAlias.Id).IsInG(bookIdList);

                return dbQuery
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetLastTypeaheadHeadwords(int recordCount, IList<long> selectedBookIds = null)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;

                var dbQuery = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookHeadwordAlias.Headword)))
                    .Where(x => x.Visibility == VisibilityEnum.Public);

                if (selectedBookIds != null)
                    dbQuery.AndRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                return dbQuery
                    .Take(recordCount)
                    .List<string>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<string> GetTypeaheadHeadwords(string query, int recordCount, IList<long> selectedBookIds = null)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;
                
                var dbQuery = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookHeadwordAlias.Headword)))
                    .Where(x => x.Visibility == VisibilityEnum.Public)
                    .AndRestrictionOn(x => x.Headword).IsInsensitiveLike(query);

                if (selectedBookIds != null)
                    dbQuery.AndRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                return dbQuery
                    .Take(recordCount)
                    .List<string>();
            }
        }
    }
}