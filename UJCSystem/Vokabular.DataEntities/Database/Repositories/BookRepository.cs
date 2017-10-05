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