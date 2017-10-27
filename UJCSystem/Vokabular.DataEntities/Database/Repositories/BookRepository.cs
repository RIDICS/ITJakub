using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class BookRepository : NHibernateDao
    {
        public BookRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
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
                .Where(x => x.ParentResource == null && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .AndRestrictionOn(() => projectAlias.Id).IsInG(projectIdList)
                .OrderBy(() => projectAlias.Id).Asc
                .OrderBy(x => x.AudioType).Asc
                .List();

            return result;
        }

        public IList<AudioResource> GetRecordings(long projectId)
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<AudioResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .Fetch(x => x.Resource).Eager
                .OrderBy(x => x.ParentResource).Asc
                .OrderBy(x => x.AudioType).Asc
                .List();

            return result;
        }

        public IList<TrackResource> GetTracks(long projectId)
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            var result = GetSession().QueryOver<TrackResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .Fetch(x => x.Resource).Eager
                .OrderBy(x => x.Position).Asc
                .List();

            return result;
        }

        public virtual IList<PageResource> GetPageList(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<PageResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<ChapterResource> GetChapterList(long projectId)
        {
            Resource resourceAlias = null;

            return GetSession().QueryOver<ChapterResource>()
                .JoinAlias(x => x.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Project.Id == projectId)
                .OrderBy(x => x.Position).Asc
                .List();
        }

        public virtual IList<Term> GetPageTermList(long resourcePageId)
        {
            PageResource pageResourceAlias = null;
            Resource resourceAlias = null;

            return GetSession().QueryOver<Term>()
                .JoinAlias(x => x.PageResources, () => pageResourceAlias)
                .JoinAlias(() => pageResourceAlias.Resource, () => resourceAlias)
                .Where(() => resourceAlias.Id == resourcePageId)
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
                .Where(() => projectAlias.Id == projectId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
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
                .Where(x => x.ParentResource.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .OrderBy(x => x.CreateTime).Desc
                .Fetch(x => x.BookVersion).Eager
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
                .Where(x => x.ParentResource.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .OrderBy(x => x.CreateTime).Desc
                .List();
        }

        public virtual IList<string> GetHeadwordAutocomplete(string queryString, BookTypeEnum? bookType, IList<int> selectedCategoryIds, IList<long> selectedProjectIds, int count)
        {
            queryString = EscapeQuery(queryString);

            HeadwordResource headwordResourceAlias = null;
            Resource resourceAlias = null;
            Project projectAlias = null;
            Snapshot snapshotAlias = null;
            BookType bookTypeAlias = null;
            Category categoryAlias = null;

            var query = GetSession().QueryOver<HeadwordItem>()
                .JoinAlias(x => x.HeadwordResource, () => headwordResourceAlias)
                .JoinAlias(() => headwordResourceAlias.Resource, () => resourceAlias)
                .JoinAlias(() => resourceAlias.Project, () => projectAlias)
                .JoinAlias(() => projectAlias.LatestPublishedSnapshot, () => snapshotAlias)
                .Where(() => headwordResourceAlias.Id == resourceAlias.LatestVersion.Id)
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
        
        public virtual long GetHeadwordRowNumber(string queryString, IList<long> selectedProjectIds, IList<int> selectedCategoryIds, BookTypeEnum bookType)
        {
            IQuery query;

            if (selectedProjectIds.Count == 0 && selectedCategoryIds.Count == 0)
            {
                query = GetSession().GetNamedQuery("GetHeadwordRowNumber");
            }
            else if (selectedCategoryIds.Count == 0)
            {
                query = GetSession().GetNamedQuery("GetHeadwordRowNumberByProject")
                    .SetParameterList("projectIds", selectedProjectIds);
            }
            else
            {
                query = GetSession().GetNamedQuery("GetHeadwordRowNumberByProjectAndCategory")
                    .SetParameterList("projectIds", selectedProjectIds)
                    .SetParameterList("categoryIds", selectedCategoryIds);
            }

            queryString = $"{EscapeQuery(queryString)}%";

            var result = query
                .SetParameter("query", queryString)
                .SetParameter("bookType", bookType)
                .UniqueResult<long>();

            return result;
        }
    }
}