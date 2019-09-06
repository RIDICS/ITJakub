using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
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
        
        public Snapshot GetSnapshotWithResources(long snapshotId)
        {
            var result = GetSession().QueryOver<Snapshot>()
                .Where(x => x.Id == snapshotId)
                .Fetch(SelectMode.Fetch, x => x.BookTypes)
                .Fetch(SelectMode.Fetch, x => x.DefaultBookType)
                .SingleOrDefault();
            return result;
        }

        public ListWithTotalCountResult<Snapshot> GetPublishedSnapshots(long projectId, int start, int count, string filterByComment = null)
        {
            var query = GetSession().QueryOver<Snapshot>()
                .Where(x => x.Project.Id == projectId)
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser);
           
            if (!string.IsNullOrEmpty(filterByComment))
            {
                query.WhereRestrictionOn(x => x.Comment).IsInsensitiveLike(filterByComment, MatchMode.Anywhere);
            }

            query.OrderBy(x => x.VersionNumber).Desc
                .Skip(start)
                .Take(count);

            var list = query.Future();
            var totalCount = query.ToRowCountQuery().FutureValue<int>();

            return new ListWithTotalCountResult<Snapshot>
            {
                List = list.ToList(),
                Count = totalCount.Value
            };
        }

        public IList<SnapshotAggregatedInfo> GetSnapshotsResourcesCount(long[] snapshotIds)
        {
            ResourceVersion resourceVersionAlias = null;
            Resource resourceAlias = null;
            SnapshotAggregatedInfo contract = null;

            var result = GetSession().QueryOver<Snapshot>()
                .JoinAlias(x => x.ResourceVersions, () => resourceVersionAlias)
                .JoinAlias(() => resourceVersionAlias.Resource, () => resourceAlias)
                .Where(x => x.Id.IsIn(snapshotIds))
                .SelectList(list => 
                    list.SelectGroup(x => x.Id).WithAlias(() => contract.Id)
                    .SelectGroup(() => resourceAlias.ResourceType).WithAlias(() => contract.Type)
                    .SelectCount(() => resourceVersionAlias.Id).WithAlias(() => contract.ResourcesCount))
                      
                .TransformUsing(Transformers.AliasToBean<SnapshotAggregatedInfo>())
                .List<SnapshotAggregatedInfo>(); 
           
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