using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.Shared.Contracts.Searching;
using log4net;
using NHibernate.Criterion;
using NHibernate.Transform;
using ResponsibleType = ITJakub.DataEntities.Database.Entities.ResponsibleType;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class BookVersionRepository : NHibernateTransactionalDao
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BookVersionRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual long Create(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                return (long) session.Save(bookVersion);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void Delete(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                session.Delete(bookVersion);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion FindBookVersionByGuid(string bookVersionGuid)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<BookVersion>()
                        .Where(bookVersion => bookVersion.VersionId == bookVersionGuid)
                        .SingleOrDefault<BookVersion>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Publisher FindPublisherByText(string text)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<Publisher>()
                        .Where(publisher => publisher.Text == text)
                        .SingleOrDefault<Publisher>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Responsible FindResponsible(string text, ResponsibleType type)
        {
            using (var session = GetSession())
            {
                var responsibleType = FindResponsibleType(type) ?? type;
                return
                    session.QueryOver<Responsible>()
                        .Where(
                            responsible =>
                                responsible.Text == text && responsible.ResponsibleType.Id == responsibleType.Id)
                        .Take(1)
                        .SingleOrDefault<Responsible>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual ResponsibleType FindResponsibleType(ResponsibleType responsibleType)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<ResponsibleType>()
                        .Where(
                            respType =>
                                respType.Text == responsibleType.Text ||
                                respType.Type == responsibleType.Type)
                        .Take(1)
                        .SingleOrDefault<ResponsibleType>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage GetPageByVersionAndXmlId(BookVersion bookVersion, string pageXmlId)
        {
            using (var session = GetSession())
            {
                var bookPage =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == bookVersion.Id && x.XmlId == pageXmlId)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage FindBookPageByVersionAndPosition(BookVersion bookVersion, int position)
        {
            using (var session = GetSession())
            {
                var bookPage =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == bookVersion.Id && x.Position == position)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage FindBookPageByVersionAndXmlId(long versionId, string xmlId)
        {
            using (var session = GetSession())
            {
                var bookPage =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == versionId && x.XmlId == xmlId)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookPage> GetPageList(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                var bookPages =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == bookVersion.Id)
                        .List<BookPage>();
                return bookPages;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookContentItem> GetRootBookContentItemsWithPagesAndAncestors(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                var bookContentItems =
                    session.QueryOver<BookContentItem>()
                        //.Fetch(x => x.Page).Eager
                        .Where(item => item.BookVersion.Id == bookVersion.Id && item.ParentBookContentItem == null)
                        .List<BookContentItem>();
                return bookContentItems;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage GetPageByXmlId(string bookId, string pageXmlId)
        {
            using (var session = GetSession())
            {
                BookPage page = null;
                BookVersion version = null;

                var resultPage = session.QueryOver(() => page)
                    .JoinQueryOver(x => x.BookVersion, () => version)
                    .JoinQueryOver(x => x.Book)
                    .Where(book => book.Guid == bookId && version.Id == book.LastVersion.Id && page.XmlId == pageXmlId)
                    .SingleOrDefault<BookPage>();

               

                return resultPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersionPairContract> SearchByCriteriaQuery(List<SearchCriteriaQuery> criteriaQueries)
        {
            var queryString = "select b.Guid as Guid, min(bv.VersionId) as VersionId from Book b inner join b.LastVersion bv";
            var joinBuilder = new StringBuilder();
            var whereBuilder = new StringBuilder();
            foreach (var criteriaQuery in criteriaQueries)
            {
                if (!string.IsNullOrEmpty(criteriaQuery.Join))
                    joinBuilder.Append(' ').Append(criteriaQuery.Join);

                whereBuilder.Append(whereBuilder.Length > 0 ? " and" : " where");

                whereBuilder.Append(" (").Append(criteriaQuery.Where).Append(')');
            }

            queryString = string.Format("{0}{1}{2} group by b.Guid", queryString, joinBuilder, whereBuilder);

            using (var session = GetSession())
            {
                var paramIndex = 0;
                var query = session.CreateQuery(queryString);
                foreach (var criteriaQuery in criteriaQueries)
                {
                    foreach (var parameterValue in criteriaQuery.Parameters)
                    {
                        if (parameterValue is DateTime)
                            //set parameter as DateTime2 otherwise comparison years before 1753 doesn't work
                            query.SetDateTime2(paramIndex, (DateTime)parameterValue);
                        else
                            query.SetParameter(paramIndex, parameterValue);

                        paramIndex++;
                    }
                }
                
                var result = query.SetResultTransformer(Transformers.AliasToBean<BookVersionPairContract>())
                    .List<BookVersionPairContract>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> GetBookVersionsByGuid(IEnumerable<string> bookGuidList)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                return session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .Select(x => x.LastVersion)
                    .WhereRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                    .List<BookVersion>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HeadwordSearchResult> SearchHeadword(string query, IList<string> dictionaryGuidList, int page, int pageSize)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;
                HeadwordSearchResult resultAlias = null;

                var subquery = QueryOver.Of(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.Property(() => bookHeadwordAlias.Headword)))
                    .WhereRestrictionOn(x => x.Headword).IsInsensitiveLike(query)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(dictionaryGuidList)
                    .OrderBy(x => x.Headword).Asc
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                var result = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .WithSubquery
                    .WhereProperty(x => x.Headword).In(subquery)
                    .SelectList(list => list
                        .Select(x => bookAlias.Guid).WithAlias(() => resultAlias.BookGuid)
                        .Select(x => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle)
                        .Select(x => bookVersionAlias.Acronym).WithAlias(() => resultAlias.BookAcronym)
                        .Select(x => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword)
                        .Select(x => bookHeadwordAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))
                    .OrderBy(x => x.DefaultHeadword).Asc
                    .TransformUsing(Transformers.AliasToBean<HeadwordSearchResult>())
                    .List<HeadwordSearchResult>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetHeadwordCount(IList<long> selectedBookIds = null)
        {
            Book bookAlias = null;
            BookHeadword bookHeadwordAlias = null;

            using (var session = GetSession())
            {
                var query = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.CountDistinct(() => bookHeadwordAlias.XmlEntryId));

                if (selectedBookIds != null)
                    query.WhereRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                return query.SingleOrDefault<int>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HeadwordSearchResult> GetHeadwordList(int page, int pageSize, IList<long> selectedBookIds = null)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;
                HeadwordSearchResult resultAlias = null;

                var query = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias);

                if (selectedBookIds != null)
                    query.WhereRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                var result = query.Select(Projections.Distinct(Projections.ProjectionList()
                        .Add(Projections.Property(() => bookAlias.Guid).WithAlias(() => resultAlias.BookGuid))
                        .Add(Projections.Property(() => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle))
                        .Add(Projections.Property(() => bookVersionAlias.Acronym).WithAlias(() => resultAlias.BookAcronym))
                        .Add(Projections.Property(() => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword))
                        .Add(Projections.Property(() => bookHeadwordAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))))
                    .OrderBy(x => x.DefaultHeadword).Asc
                    .TransformUsing(Transformers.AliasToBean<HeadwordSearchResult>())
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .List<HeadwordSearchResult>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetPageNumberForHeadword(IList<long> selectedBookIds, string headwordQuery, int pageSize)
        {
            using (var session = GetSession())
            {

                return 2; //TODO
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetCountOfSearchHeadword(string query, IList<string> dictionaryGuidList)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;

                var result = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.CountDistinct(() => bookHeadwordAlias.DefaultHeadword))
                    .WhereRestrictionOn(x => x.Headword).IsInsensitiveLike(query)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(dictionaryGuidList)
                    .SingleOrDefault<int>();

                return result;
            }
        }
    }
}