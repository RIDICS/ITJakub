using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Entities.SelectResults;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace ITJakub.DataEntities.Database.Repositories
{
    public class BookVersionRepository : NHibernateTransactionalDao
    {
        public BookVersionRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual long Create(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                return (long) session.Save(bookVersion);
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual void Delete(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                session.Delete(bookVersion);
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual Responsible FindResponsible(string text, ResponsibleType respType)
        {
            using (var session = GetSession())
            {
                var responsibleType = FindResponsibleType(respType.Text, respType.Type) ?? respType;
                return
                    session.QueryOver<Responsible>()
                        .Where(
                            responsible =>
                                responsible.Text == text && responsible.ResponsibleType.Id == responsibleType.Id)
                        .SingleOrDefault<Responsible>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual ResponsibleType FindResponsibleType(string responsibleTypeText,
            ResponsibleTypeEnum responsibleTypeEnum)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<ResponsibleType>()
                        .Where(
                            respType =>
                                respType.Text == responsibleTypeText &&
                                respType.Type == responsibleTypeEnum)
                        .SingleOrDefault<ResponsibleType>();
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual void CreateResponsibleTypeIfNotExist(ResponsibleType responsibleType)
        {
            using (var session = GetSession())
            {
                var tmpResponsibleType = FindResponsibleType(responsibleType.Text, responsibleType.Type);
                if (tmpResponsibleType == null)
                {
                    session.Save(responsibleType);
                }
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual int CountBookPageByXmlId(string bookXmlId, string versionId)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookPages =
                    session.QueryOver<BookPage>()
                        .JoinAlias(bookPage => bookPage.BookVersion, () => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(
                            bookPage =>
                                bookAlias.Guid == bookXmlId && bookVersionAlias.VersionId == versionId &&
                                bookPage.XmlResource != null)
                        .RowCount();
                return bookPages;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual int CountBookImageByXmlId(string bookXmlId, string versionId)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookImages =
                    session.QueryOver<BookPage>()
                        .JoinAlias(bookPage => bookPage.BookVersion, () => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(
                            bookPage =>
                                bookAlias.Guid == bookXmlId && bookVersionAlias.VersionId == versionId &&
                                bookPage.Image != null)
                        .RowCount();
                return bookImages;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual BookPage FindBookPageByXmlIdAndPosition(string bookXmlId, int position)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;

                var bookPage =
                    session.QueryOver<BookPage>()
                        .JoinAlias(page => page.BookVersion, () => bookVersionAlias)
                        .JoinAlias(() => bookVersionAlias.Book, () => bookAlias)
                        .Where(
                            page =>
                                bookAlias.Guid == bookXmlId && bookAlias.LastVersion.Id == bookVersionAlias.Id &&
                                page.Position == position)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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
                        .Where(
                            x =>
                                bookAlias.Guid == bookXmlId && bookAlias.LastVersion.Id == bookVersionAlias.Id &&
                                x.XmlId == xmlId)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual BookPage GetPageByXmlId(string bookXmlId, string pageXmlId)
        {
            using (var session = GetSession())
            {
                BookPage page = null;
                BookVersion version = null;

                var resultPage = session.QueryOver(() => page)
                    .JoinQueryOver(x => x.BookVersion, () => version)
                    .JoinQueryOver(x => x.Book)
                    .Where(
                        book => book.Guid == bookXmlId && version.Id == book.LastVersion.Id && page.XmlId == pageXmlId)
                    .SingleOrDefault<BookPage>();


                return resultPage;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<BookVersionPairContract> SearchByCriteriaQuery(SearchCriteriaQueryCreator creator)
        {
            using (var session = GetSession())
            {
                var query = session.CreateQuery(creator.GetQueryStringForBookVersionPair());
                creator.SetParameters(query);
                var result =
                    query.SetResultTransformer(Transformers.AliasToBean<BookVersionPairContract>())
                        .List<BookVersionPairContract>();
                return result;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<Track> GetTracksForBookVersion(long bookVersionId)
        {
            using (var session = GetSession())
            {
                Track trackAlias = null;

                var query = session.QueryOver(() => trackAlias)
                    .Where(() => trackAlias.BookVersion.Id == bookVersionId)
                    .Future<Track>();

                session.QueryOver(() => trackAlias)
                    .Fetch(x => x.Recordings).Eager
                    .Future<Track>();

                var result = query.ToList();
                return result;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<BookVersion> GetBookVersionDetailsByGuid(IList<string> bookGuidList, int? start, int? count,
            SortEnum? sorting, ListSortDirection? direction)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                ManuscriptDescription manuscriptDescriptionAlias = null;
                Author authorAlias = null;

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
                                    .JoinAlias(x => x.ManuscriptDescriptions, () => manuscriptDescriptionAlias,
                                        JoinType.LeftOuterJoin)
                                    .OrderBy(() => manuscriptDescriptionAlias.NotAfter).Desc;
                            }
                            else
                            {
                                query = query
                                    .JoinAlias(x => x.ManuscriptDescriptions, () => manuscriptDescriptionAlias,
                                        JoinType.LeftOuterJoin)
                                    .OrderBy(() => manuscriptDescriptionAlias.NotBefore).Asc;
                            }
                            break;
                        case SortEnum.Author:
                            queryOrder = query
                                .JoinAlias(x => x.Authors, () => authorAlias, JoinType.LeftOuterJoin)
                                .OrderBy(() => authorAlias.Name);
                            query = SetOrderDirection(queryOrder, direction.Value);
                            break;
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
                    .Future<BookVersion>();

                if (sorting == null || sorting.Value != SortEnum.Dating)
                {
                    session.QueryOver(() => bookVersionAlias)
                        .JoinAlias(x => x.Book, () => bookAlias)
                        .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                        .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                        .Fetch(x => x.ManuscriptDescriptions).Eager
                        .Future<BookVersion>();
                }

                if (sorting == null || sorting.Value != SortEnum.Author)
                {
                    session.QueryOver(() => bookVersionAlias)
                        .JoinAlias(x => x.Book, () => bookAlias)
                        .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                        .AndRestrictionOn(x => bookAlias.Guid).IsInG(bookGuidList)
                        .Fetch(x => x.Authors).Eager
                        .Future<BookVersion>();
                }
                
                var result = futureResult.ToList();
                return result;
            }
        }

        private IQueryOver<BookVersion, BookVersion> SetOrderDirection(
            IQueryOverOrderBuilder<BookVersion, BookVersion> queryOrder,
            ListSortDirection direction)
        {
            return direction == ListSortDirection.Descending
                ? queryOrder.Desc
                : queryOrder.Asc;
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual BookVersion GetBookVersionDetailByBookId(long bookId)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                Responsible responsibleAlias = null;
                ResponsibleType responsibleTypeAlias = null;
                LiteraryOriginal literaryOriginalAlias = null;
                LiteraryKind literaryKindAlias = null;
                LiteraryGenre genreAlias = null;

                var query = session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId);

                var futureResult = query
                    .Fetch(x => x.Book).Eager
                    .Fetch(x => x.Publisher).Eager
                    .Fetch(x => x.DefaultBookType).Eager
                    .FutureValue<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Fetch(x => x.ManuscriptDescriptions).Eager
                    .FutureValue<BookVersion>();
                
                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Fetch(x => x.Authors).Eager
                    .FutureValue<BookVersion>();
                
                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Fetch(x => x.Keywords).Eager
                    .FutureValue<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Fetch(x => x.FullBookRecordings).Eager
                    .FutureValue<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Left.JoinAlias(() => bookVersionAlias.Responsibles, () => responsibleAlias)
                    .Left.JoinAlias(() => responsibleAlias.ResponsibleType, () => responsibleTypeAlias)
                    .FutureValue<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Left.JoinAlias(() => bookVersionAlias.LiteraryOriginals, () => literaryOriginalAlias)
                    .Fetch(x => x.LiteraryOriginals).Eager
                    .FutureValue<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Left.JoinAlias(() => bookVersionAlias.LiteraryKinds, () => literaryKindAlias)
                    .Fetch(x => x.LiteraryKinds).Eager
                    .FutureValue<BookVersion>();

                session.QueryOver(() => bookVersionAlias)
                    .JoinAlias(x => x.Book, () => bookAlias)
                    .Where(() => bookAlias.LastVersion.Id == bookVersionAlias.Id)
                    .And(x => bookAlias.Id == bookId)
                    .Left.JoinAlias(() => bookVersionAlias.LiteraryGenres, () => genreAlias)
                    .Fetch(x => x.LiteraryGenres).Eager
                    .FutureValue<BookVersion>();

                var result = futureResult.Value;
                return result;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<TermResult> GetBooksTermResults(List<string> bookGuidList,
            TermCriteriaQueryCreator queryCreator)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookPage bookPageAlias = null;
                Term termAlias = null;
                TermResult termResultAlias = null;

                var result = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookPages, () => bookPageAlias)
                    .JoinQueryOver(x => x.Terms, () => termAlias)
                    .SelectList(list => list
                        .Select(() => bookAlias.Id).WithAlias(() => termResultAlias.BookId)
                        .Select(() => bookPageAlias.Text).WithAlias(() => termResultAlias.PageName)
                        .Select(() => bookPageAlias.XmlId).WithAlias(() => termResultAlias.PageXmlId))
                    .OrderBy(() => bookAlias.Id).Asc
                    .OrderBy(() => bookPageAlias.Position).Asc
                    .WhereRestrictionOn(() => bookAlias.Guid).IsInG(bookGuidList)
                    .And(queryCreator.GetCondition())
                    .TransformUsing(Transformers.AliasToBean<TermResult>())
                    .List<TermResult>();

                return result;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<TermCountResult> GetBooksTermResultsCount(List<string> bookGuidList,
            TermCriteriaQueryCreator queryCreator)
        {
            using (var session = GetSession())
            {
                Book bookAlias = null;
                BookVersion bookVersionAlias = null;
                BookPage bookPageAlias = null;
                Term termAlias = null;
                TermCountResult termResultAlias = null;

                var result = session.QueryOver(() => bookAlias)
                    .JoinQueryOver(x => x.LastVersion, () => bookVersionAlias)
                    .JoinQueryOver(x => x.BookPages, () => bookPageAlias)
                    .JoinQueryOver(x => x.Terms, () => termAlias)
                    .Select(Projections.ProjectionList()
                        .Add(Projections.Group(() => bookAlias.Id).WithAlias(() => termResultAlias.BookId))
                        .Add(
                            Projections.CountDistinct(() => bookPageAlias.Id)
                                .WithAlias(() => termResultAlias.PagesCount))
                    )
                    .WhereRestrictionOn(() => bookAlias.Guid).IsInG(bookGuidList)
                    .And(queryCreator.GetCondition())
                    .TransformUsing(Transformers.AliasToBean<TermCountResult>())
                    .List<TermCountResult>();

                return result;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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
                        .Add(
                            Projections.CountDistinct(() => bookHeadwordAlias.XmlEntryId)
                                .WithAlias(() => headwordCountAlias.HeadwordCount))
                        .Add(Projections.Group(() => bookAlias.Id).WithAlias(() => headwordCountAlias.BookId))
                    );

                if (selectedBookIds != null)
                    query.WhereRestrictionOn(() => bookAlias.Id).IsInG(selectedBookIds);

                var resultList = query.TransformUsing(Transformers.AliasToBean<HeadwordCountResult>())
                    .List<HeadwordCountResult>();

                return (int) resultList.Sum(x => x.HeadwordCount);
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<HeadwordSearchResult> GetHeadwordList(int start, int count,
            IList<long> selectedBookIds = null)
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
                    .Add(
                        Projections.Property(() => bookVersionAlias.VersionId)
                            .WithAlias(() => resultAlias.BookVersionId))
                    .Add(Projections.Property(() => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle))
                    .Add(Projections.Property(() => bookVersionAlias.Acronym).WithAlias(() => resultAlias.BookAcronym))
                    .Add(
                        Projections.Property(() => bookHeadwordAlias.DefaultHeadword)
                            .WithAlias(() => resultAlias.Headword))
                    .Add(Projections.Property(() => bookHeadwordAlias.XmlEntryId)
                        .WithAlias(() => resultAlias.XmlEntryId))
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual int GetHeadwordCountBySearchCriteria(IEnumerable<string> selectedGuidList,
            HeadwordCriteriaQueryCreator creator)
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
                        .Add(
                            Projections.CountDistinct(() => bookHeadwordAlias.XmlEntryId)
                                .WithAlias(() => headwordCountAlias.HeadwordCount))
                        .Add(Projections.Group(() => bookAlias.Id).WithAlias(() => headwordCountAlias.BookId))
                    )
                    .WhereRestrictionOn(() => bookAlias.Guid).IsInG(selectedGuidList)
                    .And(creator.GetCondition())
                    .TransformUsing(Transformers.AliasToBean<HeadwordCountResult>())
                    .List<HeadwordCountResult>();

                return (int) resultList.Sum(x => x.HeadwordCount);
            }
        }

        [Transaction(TransactionScopeOption.Required)]
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
                        .Add(
                            Projections.Property(() => bookVersionAlias.VersionId)
                                .WithAlias(() => resultAlias.BookVersionId))
                        .Add(Projections.Property(() => bookVersionAlias.Title).WithAlias(() => resultAlias.BookTitle))
                        .Add(
                            Projections.Property(() => bookVersionAlias.Acronym)
                                .WithAlias(() => resultAlias.BookAcronym))
                        .Add(
                            Projections.Property(() => bookHeadwordAlias.DefaultHeadword)
                                .WithAlias(() => resultAlias.Headword))
                        .Add(
                            Projections.Property(() => bookHeadwordAlias.SortOrder)
                                .WithAlias(() => resultAlias.SortOrder))
                        .Add(
                            Projections.Property(() => bookHeadwordAlias.XmlEntryId)
                                .WithAlias(() => resultAlias.XmlEntryId))
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual long GetHeadwordRowNumberById(IList<long> selectedBookIds, string headwordBookGuid,
            string headwordEntryXmlId)
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<HeadwordSearchResult> SearchHeadwordByCriteria(SearchCriteriaQueryCreator creator,
            int start, int count)
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual TrackRecording GetRecordingByTrackAndAudioType(long bookId, long trackPosition,
            AudioType audioType)
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
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

        [Transaction(TransactionScopeOption.Required)]
        public virtual IList<Term> GetTermsOnPage(string bookXmlId, string pageXmlId)
        {
            using (var session = GetSession())
            {
                Term term = null;
                BookPage page = null;
                BookVersion version = null;

                var terms = session.QueryOver(() => term)
                    .JoinQueryOver(x => term.ReferencedFrom, () => page)
                    .JoinQueryOver(x => page.BookVersion, () => version)
                    .JoinQueryOver(x => version.Book)
                    .Where(
                        book => book.Guid == bookXmlId && version.Id == book.LastVersion.Id && page.XmlId == pageXmlId)
                    .OrderBy(() => term.Position).Asc
                    .List<Term>();

                return terms;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual BookType GetBookTypeByBookVersionId(long bookVersionId)
        {
            BookVersion bookVersionAlias = null;

            using (var session = GetSession())
            {
                var result = session.QueryOver<BookType>()
                    .JoinAlias(x => x.BookVersions, () => bookVersionAlias)
                    .Where(x => bookVersionAlias.Id == bookVersionId)
                    .SingleOrDefault();

                return result;
            }
        }

        [Transaction(TransactionScopeOption.Required)]
        public virtual BookType GetBookType(BookTypeEnum bookType)
        {
            using (var session = GetSession())
            {
                var result = session.QueryOver<BookType>()
                    .Where(x => x.Type == bookType)
                    .SingleOrDefault();

                return result;
            }
        }
    }
}