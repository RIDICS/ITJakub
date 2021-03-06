﻿using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class BookViewRepository : MainDbRepositoryBase
    {
        public BookViewRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual ProjectIdentificationResult GetProjectIdentification(long projectId)
        {
            ProjectIdentificationResult resultAlias = null;
            Snapshot snapshotAlias = null;
            BookVersionResource bookVersionResourceAlias = null;

            var dbResult = GetSession().QueryOver<Project>()
                .JoinAlias(x => x.LatestPublishedSnapshot, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.BookVersion, () => bookVersionResourceAlias, JoinType.LeftOuterJoin)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => resultAlias.ProjectId)
                    .Select(x => x.ExternalId).WithAlias(() => resultAlias.ProjectExternalId)
                    .Select(() => snapshotAlias.Id).WithAlias(() => resultAlias.SnapshotId)
                    .Select(() => bookVersionResourceAlias.ExternalId).WithAlias(() => resultAlias.BookVersionExternalId)
                    .Select(x => x.ProjectType).WithAlias(() => resultAlias.ProjectType)
                )
                .Where(x => x.Id == projectId && x.IsRemoved == false)
                .TransformUsing(Transformers.AliasToBean<ProjectIdentificationResult>())
                .SingleOrDefault<ProjectIdentificationResult>();

            return dbResult;
        }

        public virtual IList<AudioResource> GetFullBookRecordings(IEnumerable<long> projectIdList)
        {
            Resource resourceAlias = null;
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<AudioResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(x => x.ResourceTrack == null && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(projectIdList)
                .OrderBy(() => projectAlias.Id).Asc
                .OrderBy(x => x.AudioType).Asc
                .List();

            return result;
        }

        public virtual IList<AudioResource> GetRecordings(long projectId)
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<AudioResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .OrderBy(x => x.ResourceTrack).Asc
                .OrderBy(x => x.AudioType).Asc
                .List();

            return result;
        }

        public virtual IList<TrackResource> GetTracks(long projectId)
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<TrackResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .OrderBy(x => x.Position).Asc
                .List();

            return result;
        }

        public virtual IList<PageResource> GetPageList(long projectId)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<PageResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<PageCountResult> GetPublishedPageCount(IEnumerable<long> projectIdList)
        {
            PageResource pageResourceAlias = null;
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;
            PageCountResult resultAlias = null;

            var result = GetSession().QueryOver(() => pageResourceAlias)
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .WhereRestrictionOn(() => resourceAlias.Project.Id).IsInG(projectIdList)
                .And(() => snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .SelectList(list => list
                    .SelectGroup(() => projectAlias.Id).WithAlias(() => resultAlias.ProjectId)
                    .SelectCount(() => pageResourceAlias.Id).WithAlias(() => resultAlias.PageCount))
                .TransformUsing(Transformers.AliasToBean<PageCountResult>())
                .List<PageCountResult>();

            return result;
        }

        public virtual IList<ChapterResource> GetChapterList(long projectId)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<ChapterResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<Term> GetPageTermList(long resourcePageId)
        {
            PageResource pageResourceAlias = null;
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<Term>()
                .JoinAlias(x => x.PageResources, () => pageResourceAlias)
                .JoinAlias(() => pageResourceAlias.Snapshots, () => snapshotAlias)
                .JoinAlias(() => pageResourceAlias.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(() => resourceAlias.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual int GetPublishedResourceCount<T>(long projectId) where T : ResourceVersion
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<T>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .RowCount();
        }

        public virtual IList<TextResource> GetPageText(long resourcePageId)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;
            
            return GetSession().QueryOver<TextResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                //.JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.ResourcePage.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .OrderBy(x => x.CreateTime).Desc
                .Fetch(SelectMode.Fetch, x => x.BookVersion)
                .List();
        }

        public virtual IList<ImageResource> GetPageImage(long resourcePageId)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<ImageResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                //.JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.ResourcePage.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .OrderBy(x => x.CreateTime).Desc
                .List();
        }

        public virtual HeadwordResource GetHeadwordResource(long resourceId, bool fetchHeadwordItems)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            var query = GetSession().QueryOver<HeadwordResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.Resource.Id == resourceId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .Fetch(SelectMode.Fetch, x => x.BookVersion);

            if (fetchHeadwordItems)
            {
                query = query.Fetch(SelectMode.Fetch, x => x.HeadwordItems);
            }
                
            return query.SingleOrDefault();
        }

        public virtual T GetPublishedResourceVersion<T>(long resourceId) where T : ResourceVersion
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<T>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Where(x => x.Resource.Id == resourceId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id && projectAlias.IsRemoved == false)
                .SingleOrDefault();
        }

        public virtual IList<string> GetHeadwordAutocomplete(string queryString, ProjectTypeEnum projectType, BookTypeEnum? bookType,
            IList<int> selectedCategoryIds, IList<long> selectedProjectIds, int count, int userId)
        {
            queryString = EscapeQuery(queryString);

            HeadwordResource headwordResourceAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;
            Permission permissionAlias = null;
            UserGroup userGroupAlias = null;
            User userAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            var query = GetSession().QueryOver<HeadwordItem>()
                .JoinAlias(x => x.HeadwordResource, () => headwordResourceAlias)
                .JoinAlias(() => headwordResourceAlias.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias)
                .JoinAlias(() => projectAlias.Permissions, () => permissionAlias)
                .JoinAlias(() => permissionAlias.UserGroup, () => userGroupAlias)
                .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                .Where(() => headwordResourceAlias.Id == resourceAlias.LatestVersion.Id && !resourceAlias.IsRemoved && userAlias.Id == userId && projectAlias.ProjectType == projectType && projectAlias.IsRemoved == false)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(PermissionFlag.ShowPublished))
                .AndRestrictionOn(x => x.Headword).IsLike(queryString, MatchMode.Start)
                .Select(Projections.Distinct(Projections.Property<HeadwordItem>(x => x.Headword)))
                .OrderBy(x => x.Headword).Asc;

            if (bookType != null)
            {
                query.JoinAlias(() => snapshotAlias.BookTypes, () => bookTypeAlias)
                    .Where(() => bookTypeAlias.Type == bookType.Value);
            }

            if (selectedCategoryIds.Count > 0 || selectedProjectIds.Count > 0)
            {
                query.JoinAlias(() => projectAlias.Categories, () => categoryAlias, JoinType.LeftOuterJoin)
                    .Where(Restrictions.Or(
                        Restrictions.InG(Projections.Property(() => categoryAlias.Id), selectedCategoryIds),
                        Restrictions.InG(Projections.Property(() => projectAlias.Id), selectedProjectIds)
                    ));
            }

            return query
                .Take(count)
                .List<string>();
        }
        
        public virtual long GetHeadwordRowNumber(string queryString, IList<long> selectedProjectIds)
        {
            queryString = $"{EscapeQuery(queryString)}{WildcardAny}";

            var result = GetSession().GetNamedQuery("GetHeadwordRowNumberByProject")
                .SetParameterList("projectIds", selectedProjectIds)
                .SetParameter("query", queryString)
                .UniqueResult<long>();

            return result;
        }

        public virtual long SearchByCriteriaQueryCount(SearchCriteriaQueryCreator creator)
        {
            var session = GetSession();

            var query = session.CreateQuery(creator.GetQueryStringForCount())
                .SetParameters(creator);
            var result = query.UniqueResult<long>();
            return result;
        }

        public virtual long SearchHeadwordByCriteriaQueryCount(SearchCriteriaQueryCreator creator)
        {
            HeadwordItem headwordItemAlias = null;
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var headwordRestrictions = creator.GetHeadwordRestrictions();

            var projectIds = SearchProjectIdByCriteriaQuery(creator).Select(x => x.ProjectId);

            var query = GetSession().QueryOver<HeadwordResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(x => snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .AndRestrictionOn(x => projectAlias.Id).IsInG(projectIds);

            if (creator.HasHeadwordRestrictions())
            {
                query.JoinAlias(x => x.HeadwordItems, () => headwordItemAlias)
                    .And(headwordRestrictions);
            }

            var result = query
                .Select(Projections.CountDistinct<HeadwordResource>(x => x.Id))
                .SingleOrDefault<int>();
            return result;
        }

        public virtual IList<MetadataResource> SearchByCriteriaQuery(SearchCriteriaQueryCreator creator)
        {
            var session = GetSession();

            var query = session.CreateQuery(creator.GetQueryString())
                .SetPaging(creator)
                .SetParameters(creator);
            var result = query.List<MetadataResource>();
            return result;
        }

        public virtual IList<ProjectIdentificationResult> SearchProjectIdByCriteriaQuery(SearchCriteriaQueryCreator creator)
        {
            var session = GetSession();

            var query = session.CreateQuery(creator.GetProjectIdentificationListQueryString())
                //.SetPaging(creator) // return ALL project.Ids
                .SetParameters(creator)
                .SetResultTransformer(Transformers.AliasToBean<ProjectIdentificationResult>());
            var result = query.List<ProjectIdentificationResult>();
            return result;
        }

        public virtual IList<HeadwordSearchResult> SearchHeadwordByCriteriaQuery(SearchCriteriaQueryCreator creator)
        {
            HeadwordSearchResult resultAlias = null;
            HeadwordItem headwordItemAlias = null;
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var headwordRestrictions = creator.GetHeadwordRestrictions();

            var projectIds = SearchProjectIdByCriteriaQuery(creator).Select(x => x.ProjectId);

            var query = GetSession().QueryOver<HeadwordResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(x => snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .AndRestrictionOn(x => projectAlias.Id).IsInG(projectIds);

            if (creator.HasHeadwordRestrictions())
            {
                query.JoinAlias(x => x.HeadwordItems, () => headwordItemAlias)
                    .And(headwordRestrictions);
            }

            var result = query
                .SelectList(list => list
                    .SelectGroup(x => x.Id).WithAlias(() => resultAlias.Id)
                    .SelectMin(x => x.Sorting).WithAlias(() => resultAlias.Sorting))
                .OrderBy(x => x.Sorting).Asc
                .TransformUsing(Transformers.AliasToBean<HeadwordSearchResult>())
                .Take(creator.GetCount())
                .Skip(creator.GetStart())
                .List<HeadwordSearchResult>();
            return result;
        }

        public virtual IList<PageResource> GetPagesWithTerms(TermCriteriaPageConditionCreator creator)
        {
            var query = GetSession().CreateQuery(creator.GetQueryString())
                .SetParameters(creator)
                .SetResultTransformer(Transformers.DistinctRootEntity);
            var result = query.List<PageResource>();
            return result;
        }

        public virtual IList<long> GetProjectIds(BookTypeEnum bookType, int userId, ProjectTypeEnum projectType, IList<long> projectIds,
            IList<int> categoryIds)
        {
            Project projectAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;
            Permission permissionAlias = null;
            UserGroup userGroupAlias = null;
            User userAlias = null;

            var query = GetSession().QueryOver<Snapshot>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .JoinAlias(x => x.BookTypes, () => bookTypeAlias)
                .JoinAlias(() => projectAlias.Categories, () => categoryAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => projectAlias.Permissions, () => permissionAlias)
                .JoinAlias(() => permissionAlias.UserGroup, () => userGroupAlias)
                .JoinAlias(() => userGroupAlias.Users, () => userAlias)
                .And(() => bookTypeAlias.Type == bookType && userAlias.Id == userId && projectAlias.ProjectType == projectType && projectAlias.IsRemoved == false)
                .And(BitwiseExpression.On(() => permissionAlias.Flags).HasBit(PermissionFlag.ShowPublished))
                .Select(Projections.Distinct(Projections.Property(() => projectAlias.Id)));

            if (projectIds != null && categoryIds != null)
            {
                query = query.Where(Restrictions.Or(
                    Restrictions.InG(Projections.Property(() => projectAlias.Id), projectIds),
                    Restrictions.InG(Projections.Property(() => categoryAlias.Id), categoryIds)
                ));
            }

            return query.List<long>();
        }

        public virtual IList<PageResource> GetPagesByTextVersionId(IEnumerable<long> textVersionIds)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            var subquery = QueryOver.Of<TextResource>()
                .WhereRestrictionOn(x => x.Id).IsInG(textVersionIds)
                .Select(x => x.ResourcePage.Id);

            var pageResourceIds = GetSession().QueryOver<PageResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .OrderBy(x => x.Position).Asc
                .WithSubquery
                .WhereProperty(() => resourceAlias.Id).In(subquery)
                .And(() => snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .List();

            return pageResourceIds;
        }

        public virtual IList<PageResource> GetPagesByTextExternalId(IEnumerable<string> textExternalIds, long projectId)
        {
            return GetPagesByTextExternalIdInner(textExternalIds, projectId, null, null);
        }

        public virtual IList<PageResource> GetPagesByTextExternalId(IEnumerable<string> textExternalIds, string projectExternalId, ProjectTypeEnum projectType)
        {
            return GetPagesByTextExternalIdInner(textExternalIds, null, projectExternalId, projectType);
        }

        private IList<PageResource> GetPagesByTextExternalIdInner(IEnumerable<string> textExternalIds, long? projectId, string projectExternalId, ProjectTypeEnum? projectType)
        {
            Snapshot snapshotAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;

            var subquery = QueryOver.Of<TextResource>()
                .WhereRestrictionOn(x => x.ExternalId).IsInG(textExternalIds)
                .Select(x => x.ResourcePage.Id);

            var query = GetSession().QueryOver<PageResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .Fetch(SelectMode.Fetch, x => x.Resource)
                .OrderBy(x => x.Position).Asc
                .WithSubquery
                .WhereProperty(() => resourceAlias.Id).In(subquery)
                .And(() => snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id);

            if (projectId != null)
            {
                query.And(() => projectAlias.Id == projectId.Value);
            }
            if (projectExternalId != null)
            {
                query.And(() => projectAlias.ExternalId == projectExternalId && projectAlias.ProjectType == projectType.Value);
            }

            var pageResourceIds = query.List();
            return pageResourceIds;
        }

        public virtual Transformation GetDefaultTransformation(OutputFormatEnum outputFormat, BookTypeEnum requestedBookType)
        {
            BookType bookTypeAlias = null;

            return GetSession().QueryOver<Transformation>()
                .JoinAlias(x => x.BookType, () => bookTypeAlias)
                .Where(x => x.OutputFormat == outputFormat && x.IsDefaultForBookType && bookTypeAlias.Type == requestedBookType)
                .SingleOrDefault();
        }

        public virtual IList<BookType> GetBookTypes()
        {
            return GetSession().QueryOver<BookType>()
                .OrderBy(x => x.Id).Asc
                .List();
        }
    }
}