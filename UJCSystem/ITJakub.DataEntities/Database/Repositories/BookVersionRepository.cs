using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.Shared.Contracts.Searching;
using log4net;
using NHibernate.Criterion;
using NHibernate.Transform;

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
        public virtual IList<BookVersionPairContract> SearchByCriteriaQuery(SearchCriteriaQueryCreator creator)
        {
            using (var session = GetSession())
            {
                var query = session.CreateQuery(creator.GetQueryStringForBookVersionPair());
                creator.SetParameters(query);
                var result = query.SetResultTransformer(Transformers.AliasToBean<BookVersionPairContract>()).List<BookVersionPairContract>();
                return result;
            }
        }


        //TODO inspect performance (fix lazy=false)
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
        public virtual int GetHeadwordCount(IList<long> selectedBookIds = null)
        {
            Book bookAlias = null;
            BookHeadword bookHeadwordAlias = null;
            HeadwordCountResult headwordCountAlias = null;

            using (var session = GetSession())
            {
                var query = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.ProjectionList()
                        .Add(Projections.CountDistinct(() => bookHeadwordAlias.XmlEntryId).WithAlias(() => headwordCountAlias.HeadwordCount))
                        .Add(Projections.Group(() => bookAlias.Id).WithAlias(() => headwordCountAlias.BookId))
                    );

                if (selectedBookIds != null)
                    query.WhereRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                var resultList = query.TransformUsing(Transformers.AliasToBean<HeadwordCountResult>())
                    .List<HeadwordCountResult>();

                return (int) resultList.Sum(x => x.HeadwordCount);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HeadwordSearchResult> GetHeadwordList(int start, int end, IList<long> selectedBookIds = null)
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
                    .Skip(start - 1)
                    .Take(end - start + 1)
                    .List<HeadwordSearchResult>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetHeadwordCountBySearchCriteria(IEnumerable<string> selectedGuidList, HeadwordCriteriaQueryCreator creator)
        {
            Book bookAlias = null;
            BookHeadword bookHeadwordAlias = null;
            HeadwordCountResult headwordCountAlias = null;

            using (var session = GetSession())
            {
                var resultList = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.ProjectionList()
                        .Add(Projections.CountDistinct(() => bookHeadwordAlias.XmlEntryId).WithAlias(() => headwordCountAlias.HeadwordCount))
                        .Add(Projections.Group(() => bookAlias.Id).WithAlias(() => headwordCountAlias.BookId))
                    )
                    .WhereRestrictionOn(() => bookAlias.Guid).IsInG(selectedGuidList)
                    .And(creator.GetCondition(bookHeadwordAlias))
                    .TransformUsing(Transformers.AliasToBean<HeadwordCountResult>())
                    .List<HeadwordCountResult>();

                return (int)resultList.Sum(x => x.HeadwordCount);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HeadwordSearchResult> GetHeadwordListBySearchCriteria(IEnumerable<string> selectedGuidList, HeadwordCriteriaQueryCreator creator, int start, int count)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;
                HeadwordSearchResult resultAlias = null;

                var result = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookHeadwords, () => bookHeadwordAlias)
                    .Select(Projections.Distinct(Projections.ProjectionList()
                        .Add(Projections.Property(() => bookAlias.Guid).WithAlias(() => resultAlias.BookGuid))
                        .Add(Projections.Property(() => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle))
                        .Add(Projections.Property(() => bookVersionAlias.Acronym).WithAlias(() => resultAlias.BookAcronym))
                        .Add(Projections.Property(() => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword))
                        .Add(Projections.Property(() => bookHeadwordAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))))
                    .WhereRestrictionOn(() => bookAlias.Guid).IsInG(selectedGuidList)
                    .And(creator.GetCondition(bookHeadwordAlias))
                    .OrderBy(x => x.DefaultHeadword).Asc
                    .TransformUsing(Transformers.AliasToBean<HeadwordSearchResult>())
                    .Skip(start)
                    .Take(count)
                    .List<HeadwordSearchResult>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetHeadwordRowNumber(IList<long> selectedBookIds, string headwordQuery)
        {
            using (var session = GetSession())
            {

                return 100; //TODO
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetHeadwordRowNumberById(IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId)
        {
            using (var session = GetSession())
            {
                return 200; //TODO
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual int GetSearchHeadwordCount(SearchCriteriaQueryCreator creator)
        {
            using (var session = GetSession())
            {
                var query = session.CreateQuery(creator.GetQueryStringForHeadwordCount());
                creator.SetParameters(query);
                var result = query
                    .SetResultTransformer(Transformers.AliasToBean<HeadwordCountResult>())
                    .List<HeadwordCountResult>();

                return (int) result.Sum(x => x.HeadwordCount);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HeadwordSearchResult> SearchHeadwordByCriteria(SearchCriteriaQueryCreator creator, int start, int count)
        {
            using (var session = GetSession())
            {
                var query = session.CreateQuery(creator.GetQueryStringForHeadwordList());
                creator.SetParameters(query);
                
                var result = query
                    .SetFirstResult(start)
                    .SetMaxResults(count)
                    .SetResultTransformer(Transformers.AliasToBean<HeadwordSearchResult>())
                    .List<HeadwordSearchResult>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookHeadword GetFirstHeadwordInfo(string bookXmlId, string entryXmlId)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<BookHeadword>()
                    .Fetch(x => x.BookVersion).Eager
                    .Where(x => x.XmlEntryId == entryXmlId && x.BookVersion.Book.Guid == bookXmlId)
                    .Take(1)
                    .SingleOrDefault<BookHeadword>();

                return result;
            }
        }
    }
}