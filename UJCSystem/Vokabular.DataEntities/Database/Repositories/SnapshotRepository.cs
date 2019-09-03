using NHibernate;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class SnapshotRepository : MainDbRepositoryBase
    {
        public SnapshotRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public Snapshot GetLatestPublishedSnapshot(long projectId)
        {
            Project projectAlias = null;

            var result = GetSession().QueryOver<Snapshot>()
                .JoinAlias(x => x.Project, () => projectAlias)
                .Where(x => x.Project.Id == projectId)
                .Where(x => projectAlias.LatestPublishedSnapshot.Id == x.Id)
                .Fetch(SelectMode.Fetch, x => x.BookTypes)
                .Fetch(SelectMode.Fetch, x => x.DefaultBookType)
                .SingleOrDefault();
            return result;
        }

        public Snapshot GetSnapshot(long snapshotId)
        {
            var result = GetSession().QueryOver<Snapshot>()
                .Where(x => x.Id == snapshotId)
                .Fetch(SelectMode.Fetch, x => x.Project)
                .SingleOrDefault();
            return result;
        }
    }
}