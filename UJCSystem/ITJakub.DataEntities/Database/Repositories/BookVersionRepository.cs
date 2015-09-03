using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.SqlCommand;
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
        public virtual BookPage FindBookPageByXmlIdAndPosition(string bookXmlId, int position)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookPage =
                    session.QueryOver<BookPage>()
                        .JoinAlias(x => x.BookVersion, () => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(x => bookAlias.Guid == bookXmlId && bookAlias.LastVersion.Id == bookVersionAlias.Id && x.Position == position)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage FindBookPageByXmlId(string bookXmlId, string xmlId)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookPage =
                    session.QueryOver<BookPage>()
                        .JoinAlias(x => x.BookVersion, () => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(x => bookAlias.Guid == bookXmlId && bookAlias.LastVersion.Id == bookVersionAlias.Id && x.XmlId == xmlId)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookPage> GetLastVersionPageList(string bookXmlId)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookPages =
                    session.QueryOver<BookPage>()
                        .JoinQueryOver(x => x.BookVersion, () => bookVersionAlias)
                        .JoinQueryOver(x => x.Book, () => bookAlias)
                        .Where(() => bookAlias.Guid == bookXmlId && bookAlias.LastVersion.Id == bookVersionAlias.Id)
                        .List<BookPage>();
                return bookPages;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookContentItem> GetRootBookContentItemsWithPagesAndAncestors(string bookXmlId)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookContentItems =
                    session.QueryOver<BookContentItem>()
                        .JoinAlias(x => x.BookVersion, () => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Fetch(x => x.Page).Eager
                        .Fetch(x => x.ChildContentItems).Eager
                        .Where(x => bookAlias.Guid == bookXmlId && bookAlias.LastVersion.Id == bookVersionAlias.Id)
                        .TransformUsing(Transformers.DistinctRootEntity)
                        .List<BookContentItem>()
                        .Where(x => x.ParentBookContentItem == null)
                        .ToList();

                return bookContentItems;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage GetPageByXmlId(string bookXmlId, string pageXmlId)
        {
            using (var session = GetSession())
            {
                BookPage page = null;
                BookVersion version = null;

                var resultPage = session.QueryOver(() => page)
                    .JoinQueryOver(x => x.BookVersion, () => version)
                    .JoinQueryOver(x => x.Book)
                    .Where(book => book.Guid == bookXmlId && version.Id == book.LastVersion.Id && page.XmlId == pageXmlId)
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

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> GetBookVersionsByGuid(IList<string> bookGuidList)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;

                var result = session.QueryOver<BookVersion>()
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(x => x.Id == bookAlias.LastVersion.Id)
                    .AndRestrictionOn(() => bookAlias.Guid).IsInG(bookGuidList)
                    .List<BookVersion>();

                return result;
            }
        }

        public IList<BookVersion> GetBookVersionDetailsByGuid(IList<string> bookGuidList)
        {
            return GetBookVersionDetailsByGuid(bookGuidList, null, null, null, null);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Track> GetTracksForBookVersion(long bookVersionId)
        {
            using (var session = GetSession())
            {
                Track trackAlias = null;

                var query = session.QueryOver(()=> trackAlias)
                    .Where( () => trackAlias.BookVersion.Id == bookVersionId)
                    .Future<Track>();

                session.QueryOver(() => trackAlias)
                    .Fetch(x => x.Recordings).Eager
                    .Future<Track>();

                var result = query.ToList();
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookVersion> GetBookVersionDetailsByGuid(IList<string> bookGuidList, int? start, int? count, SortEnum? sorting, ListSortDirection? direction)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                ManuscriptDescription manuscriptDescriptionAlias = null;
                Responsible responsibleAlias = null;
                ResponsibleType responsibleTypeAlias = null;

                var query = session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList);

                if (start != null && count != null)
                    query.Skip(start.Value)
                        .Take(count.Value);

                if (sorting != null && direction != null)
                {
                    IQueryOverOrderBuilder<BookVersion, BookVersion> queryOrder;
                    switch (sorting.Value)
                    {
                        case SortEnum.Title:
                            queryOrder = query.OrderBy(x => x.Title);
                            query = SetOrderDirection(queryOrder, direction.Value);
                            break;
                        case SortEnum.Dating:
                            if (direction.Value == ListSortDirection.Descending)
                            {
                                query = query
                                    .JoinAlias(x => x.ManuscriptDescriptions, () => manuscriptDescriptionAlias)
                                    .OrderBy(() => manuscriptDescriptionAlias.NotAfter).Desc;
                            }
                            else
                            {
                                query = query
                                    .JoinAlias(x => x.ManuscriptDescriptions, () => manuscriptDescriptionAlias)
                                    .OrderBy(() => manuscriptDescriptionAlias.NotBefore).Asc;
                            }
                            break;
                        // TODO order by author and editor
                        default:
                            queryOrder = query.OrderBy(x => x.Title);
                            query = SetOrderDirection(queryOrder, direction.Value);
                            break;
                    }
                }

                var futureResult = query
                    .Fetch(x => x.Book).Eager
                    .Fetch(x => x.Publisher).Eager
                    .Fetch(x => x.DefaultBookType).Eager
                    .Fetch(x => x.ManuscriptDescriptions).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                    .Fetch(x => x.Keywords).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                    .Fetch(x => x.Authors).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                    .Fetch(x => x.FullBookRecordings).Eager
                    .Future<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                    .Left.JoinAlias(() => bookVersionAlias.Responsibles, () => responsibleAlias)
                    .Left.JoinAlias(() => responsibleAlias.ResponsibleType, () => responsibleTypeAlias)
                    .Future<BookVersion>();

                var result = futureResult.ToList();
                return result;
            }
        }

        private IQueryOver<BookVersion, BookVersion> SetOrderDirection(IQueryOverOrderBuilder<BookVersion, BookVersion> queryOrder,
            ListSortDirection direction)
        {
            return direction == ListSortDirection.Descending
                ? queryOrder.Desc
                : queryOrder.Asc;
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<PageCountResult> GetBooksPageCountByGuid(IList<string> bookGuidList)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookPage bookPageAlias = null;
                PageCountResult resultAlias = null;

                var result = session.QueryOver<BookVersion>()
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .JoinAlias(x => x.BookPages, () => bookPageAlias, JoinType.LeftOuterJoin)
                    .SelectList(list => list
                        .SelectGroup(() => bookAlias.Id).WithAlias(() => resultAlias.BookId)
                        .SelectCount(() => bookPageAlias.Id).WithAlias(() => resultAlias.Count))
                    .Where(x => x.Id == bookAlias.LastVersion.Id)
                    .AndRestrictionOn(() => bookAlias.Guid).IsInG(bookGuidList)
                    .TransformUsing(Transformers.AliasToBean<PageCountResult>())
                    .List<PageCountResult>();

                return result;
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
        public virtual IList<HeadwordSearchResult> GetHeadwordList(int start, int count, IList<long> selectedBookIds = null)
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
                    .Add(Projections.Property(() => bookVersionAlias.VersionId).WithAlias(() => resultAlias.BookVersionId))
                    .Add(Projections.Property(() => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle))
                    .Add(Projections.Property(() => bookVersionAlias.Acronym).WithAlias(() => resultAlias.BookAcronym))
                    .Add(Projections.Property(() => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword))
                    .Add(Projections.Property(() => bookHeadwordAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))
                    .Add(Projections.Property(() => bookHeadwordAlias.SortOrder).WithAlias(() => resultAlias.SortOrder))
                    .Add(Projections.Property(() => bookHeadwordAlias.Image).WithAlias(() => resultAlias.Image))))
                    .OrderBy(x => x.SortOrder).Asc
                    .TransformUsing(Transformers.AliasToBean<HeadwordSearchResult>())
                    .Skip(start)
                    .Take(count)
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
                    .And(creator.GetCondition())
                    .TransformUsing(Transformers.AliasToBean<HeadwordCountResult>())
                    .List<HeadwordCountResult>();

                return (int) resultList.Sum(x => x.HeadwordCount);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<HeadwordSearchResult> GetHeadwordListBySearchCriteria(IEnumerable<string> selectedGuidList,
            HeadwordCriteriaQueryCreator creator, int start, int count)
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
                        .Add(Projections.Property(() => bookVersionAlias.VersionId).WithAlias(() => resultAlias.BookVersionId))
                        .Add(Projections.Property(() => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle))
                        .Add(Projections.Property(() => bookVersionAlias.Acronym).WithAlias(() => resultAlias.BookAcronym))
                        .Add(Projections.Property(() => bookHeadwordAlias.DefaultHeadword).WithAlias(() => resultAlias.Headword))
                        .Add(Projections.Property(() => bookHeadwordAlias.SortOrder).WithAlias(() => resultAlias.SortOrder))
                        .Add(Projections.Property(() => bookHeadwordAlias.XmlEntryId).WithAlias(() => resultAlias.XmlEntryId))
                        .Add(Projections.Property(() => bookHeadwordAlias.Image).WithAlias(() => resultAlias.Image))))
                    .WhereRestrictionOn(() => bookAlias.Guid).IsInG(selectedGuidList)
                    .And(creator.GetCondition())
                    .OrderBy(x => x.SortOrder).Asc
                    .TransformUsing(Transformers.AliasToBean<HeadwordSearchResult>())
                    .Skip(start)
                    .Take(count)
                    .List<HeadwordSearchResult>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookHeadword FindFirstHeadword(IList<long> selectedBookIds, string headwordQuery)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookHeadword bookHeadwordAlias = null;

                var query = session.QueryOver(() => bookHeadwordAlias)
                    .JoinQueryOver(() => bookHeadwordAlias.BookVersion, () => bookVersionAlias)
                    .JoinQueryOver(() => bookVersionAlias.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .AndRestrictionOn(() => bookHeadwordAlias.Headword).IsLike(headwordQuery);

                if (selectedBookIds != null)
                    query.AndRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                var result = query.OrderBy(() => bookHeadwordAlias.SortOrder).Asc
                    .Take(1)
                    .SingleOrDefault<BookHeadword>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual long GetHeadwordRowNumberById(IList<long> selectedBookIds, string headwordBookGuid, string headwordEntryXmlId)
        {
            using (var session = GetSession())
            {
                IQuery query;

                if (selectedBookIds == null)
                {
                    query = session.GetNamedQuery("GetHeadwordRowNumber");
                }
                else
                {
                    query = session.GetNamedQuery("GetHeadwordRowNumberFiltered")
                        .SetParameterList("bookIds", selectedBookIds);
                }

                var result = query
                    .SetParameter("bookGuid", headwordBookGuid)
                    .SetParameter("xmlEntryId", headwordEntryXmlId)
                    .UniqueResult<long>();

                return result;
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
        public virtual BookHeadword GetFirstHeadwordInfo(string bookXmlId, string entryXmlId, string bookVersionXmlId)
        {
            using (var session = GetSession())
            {
                BookVersion bookVersionAlias = null;
                Book bookAlias = null;

                var result = session.QueryOver<BookHeadword>()
                    .JoinAlias(x => x.BookVersion, () => bookVersionAlias)
                    .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                    .Where(x => x.XmlEntryId == entryXmlId && bookAlias.Guid == bookXmlId)
                    .And(() => bookVersionAlias.VersionId == bookVersionXmlId)
                    .Take(1)
                    .SingleOrDefault<BookHeadword>();

                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Book GetBookWithLastVersion(long bookId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Book>()
                    .Where(x => x.Id == bookId)
                    .Fetch(x => x.LastVersion).Eager
                    .SingleOrDefault<Book>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual TrackRecording GetRecordingByTrackAndAudioType(long bookId, long trackPosition, AudioType audioType)
        {
            using (var session = GetSession())
            {
                Track trackAlias = null;
                BookVersion bookVersionAlias = null;
                Book bookAlias = null;

                var trackId = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.Tracks, () => trackAlias)
                    .Where(x => trackAlias.Position == trackPosition && bookAlias.Id == bookId)
                    .Select(Projections.Property(() => trackAlias.Id))
                    .Take(1)
                    .SingleOrDefault<long>();

                return session.QueryOver<TrackRecording>()
                    .Where(x => x.Track.Id == trackId && x.AudioType == audioType)
                    .Take(1)
                    .SingleOrDefault<TrackRecording>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual FullBookRecording GetFullBookRecording(long bookVersionId, AudioType audioType)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<FullBookRecording>()
                    .Where(x => x.BookVersion.Id == bookVersionId && x.AudioType == audioType)
                    .Take(1)
                    .SingleOrDefault<FullBookRecording>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion GetBookVersionWithAuthorsByGuid(string bookGuid)
        {
            Book bookAlias = null;

            using (var session = GetSession())
            {
                var result = session.QueryOver<BookVersion>()
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(x => x.Id == bookAlias.LastVersion.Id && bookAlias.Guid == bookGuid)
                    .Fetch(x => x.Authors).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .SingleOrDefault();

                return result;
            }
        }
    }
}