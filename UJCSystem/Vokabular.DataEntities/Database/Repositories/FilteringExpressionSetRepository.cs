using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class FilteringExpressionSetRepository : NHibernateDao
    {
        public FilteringExpressionSetRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<FilteringExpression> GetFilteringExpressionsByExternalRepository(int externalRepositoryId)
        {
            FilteringExpressionSet setAlias = null;
            ExternalRepository repositoryAlias = null;

            return GetSession().QueryOver<FilteringExpression>()
                .JoinAlias(x => x.FilteringExpressionSet, () => setAlias)
                .JoinAlias(() => setAlias.ExternalRepositories, () => repositoryAlias)
                .Where(() => repositoryAlias.Id == externalRepositoryId)
                .List();
        }

        public ListWithTotalCountResult<FilteringExpressionSet> GetFilteringExpressionSetList(int start, int count)
        {
            var query = GetSession().QueryOver<FilteringExpressionSet>()
                .Fetch(x => x.BibliographicFormat).Eager;

            var list = query.Skip(start)
                .Take(count)
                .Future();

            var totalCount = query.ToRowCountQuery()
                .FutureValue<int>();

            return new ListWithTotalCountResult<FilteringExpressionSet>
            {
                List = list.ToList(),
                Count = totalCount.Value,
            };
        }

        public FilteringExpressionSet GetFilteringExpressionSet(int filteringExpressionSetId)
        {
            return GetSession().QueryOver<FilteringExpressionSet>()
                .Where(x => x.Id == filteringExpressionSetId)
                .Fetch(x => x.CreatedByUser).Eager
                .Fetch(x => x.BibliographicFormat).Eager
                .Fetch(x => x.FilteringExpressions).Eager
                .SingleOrDefault();
        }

        public IList<BibliographicFormat> GetAllBibliographicFormats()
        {
            return GetSession().QueryOver<BibliographicFormat>()
                .List();
        }
    }
}