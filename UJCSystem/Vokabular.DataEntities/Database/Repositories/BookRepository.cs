using System.Collections.Generic;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
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

        public virtual IList<TextResource> GetPageText(long resourcePageId)
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<TextResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(x => x.ParentResource.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .OrderBy(x => x.CreateTime).Desc
                .List();
        }

        public virtual IList<ImageResource> GetPageImage(long resourcePageId)
        {
            Snapshot snapshotAlias = null;
            Project projectAlias = null;

            return GetSession().QueryOver<ImageResource>()
                .JoinAlias(x => x.Snapshots, () => snapshotAlias)
                .JoinAlias(() => snapshotAlias.Project, () => projectAlias)
                .Where(x => x.ParentResource.Id == resourcePageId && snapshotAlias.Id == projectAlias.LatestPublishedSnapshot.Id)
                .OrderBy(x => x.CreateTime).Desc
                .List();
        }
    }
}