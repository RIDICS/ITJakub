using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class ExternalRepositoryRepository : MainDbRepositoryBase
    {
        public ExternalRepositoryRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
        }

        public virtual ExternalRepository GetExternalRepository(int externalRepositoryId)
        {
            return GetSession().QueryOver<ExternalRepository>()
                .Where(x => x.Id == externalRepositoryId)
                .Fetch(SelectMode.Fetch, x => x.BibliographicFormat)
                .Fetch(SelectMode.Fetch, x => x.ExternalRepositoryType)
                .Fetch(SelectMode.Fetch, x => x.FilteringExpressionSets)
                .SingleOrDefault();
        }

        public virtual IList<ExternalRepositoryType> GetAllExternalRepositoryTypes()
        {
            return GetSession().QueryOver<ExternalRepositoryType>()
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual ListWithTotalCountResult<ExternalRepository> GetExternalRepositoryList(int start, int count)
        {
            var query = GetSession().QueryOver<ExternalRepository>()
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .Fetch(SelectMode.Fetch, x => x.BibliographicFormat)
                .Fetch(SelectMode.Fetch, x => x.ExternalRepositoryType);

            var list = query.OrderBy(x => x.Name).Asc
                .Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<ExternalRepository>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }

        public IList<ExternalRepository> GetAllExternalRepositories()
        {
            return GetSession().QueryOver<ExternalRepository>()
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .Fetch(SelectMode.Fetch, x => x.BibliographicFormat)
                .Fetch(SelectMode.Fetch, x => x.ExternalRepositoryType)
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public virtual TotalImportStatistics GetExternalRepositoryStatistics(int repositoryId)
        {
            ExternalRepository externalRepository = null;
            ImportedProjectMetadata importedProjectMetadata = null;
            TotalImportStatistics totalImportStatistics = null;

            var result = GetSession().QueryOver(() => externalRepository)
                .JoinAlias(x => x.ImportedProjectMetadata, () => importedProjectMetadata, JoinType.LeftOuterJoin)
                .Where(() => externalRepository.Id == repositoryId)
                .SelectList(list => list
                    .SelectCount(() => importedProjectMetadata.Id).WithAlias(() => totalImportStatistics.NewItems))
                .TransformUsing(Transformers.AliasToBean<TotalImportStatistics>())
                .SingleOrDefault<TotalImportStatistics>();

            return result;
        }

        public virtual LastImportStatisticsResult GetLastUpdateExternalRepositoryStatistics(int repositoryId)
        {
            ExternalRepository externalRepository = null;
            ImportHistory importHistory1 = null;
            ImportHistory importHistory2 = null;
            ImportedRecordMetadata importedRecordMetadata = null;
            Snapshot snapshot = null;
            LastImportStatisticsResult lastImportStatisticsResult = null;

            var subQuery = QueryOver.Of(() => importHistory2).Where(x => x.ExternalRepository.Id == repositoryId)
                .OrderBy(x => x.Date).Desc
                .Select(x => x.Id)
                .Take(1);

            var result = GetSession().QueryOver(() => importHistory1)
                .JoinAlias(() => importHistory1.ExternalRepository, () => externalRepository)
                .JoinAlias(() => importHistory1.ImportedRecordMetadata, () => importedRecordMetadata, JoinType.LeftOuterJoin)
                .JoinAlias(() => importedRecordMetadata.Snapshot, () => snapshot, JoinType.LeftOuterJoin)
                .WithSubquery
                .WhereProperty(() => importHistory1.Id).Eq(subQuery)
                .SelectList(list => list
                    .SelectCount(() => importedRecordMetadata.Id)
                    .WithAlias(() => lastImportStatisticsResult.TotalItems)
                    .Select(
                        Projections.Sum(Projections.Conditional(
                            Restrictions.Eq(
                                Projections.Property(() => snapshot.VersionNumber), 1),
                            Projections.Constant(1, NHibernateUtil.Int32),
                            Projections.Constant(0, NHibernateUtil.Int32))))
                    .WithAlias(() => lastImportStatisticsResult.NewItems)
                    .Select(
                        Projections.Sum(Projections.Conditional(
                                Restrictions.Gt(
                                    Projections.Property(() => snapshot.VersionNumber), 1),
                                Projections.Constant(1, NHibernateUtil.Int32),
                                Projections.Constant(0, NHibernateUtil.Int32)))
                            .WithAlias(() => lastImportStatisticsResult.UpdatedItems)))
                .TransformUsing(Transformers.AliasToBean<LastImportStatisticsResult>())
                .SingleOrDefault<LastImportStatisticsResult>();

            return result;
        }
    }
}