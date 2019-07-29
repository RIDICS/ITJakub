using System.Collections.Generic;
using System.Linq;
using NHibernate;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.DataEntities.Database.Repositories
{
    public class FilteringExpressionSetRepository : MainDbRepositoryBase
    {
        public FilteringExpressionSetRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
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
                .OrderBy(x => x.Id).Asc
                .List();
        }

        public ListWithTotalCountResult<FilteringExpressionSet> GetFilteringExpressionSetList(int start, int count)
        {
            var query = GetSession().QueryOver<FilteringExpressionSet>()
                .Fetch(SelectMode.Fetch, x => x.BibliographicFormat);

            var list = query.OrderBy(x => x.Name).Asc
                .Skip(start)
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
                .Fetch(SelectMode.Fetch, x => x.CreatedByUser)
                .Fetch(SelectMode.Fetch, x => x.BibliographicFormat)
                .Fetch(SelectMode.Fetch, x => x.FilteringExpressions)
                .SingleOrDefault();
        }

        public IList<FilteringExpressionSet> GetAllFilteringExpressionSets()
        {
            
            return GetSession().QueryOver<FilteringExpressionSet>()
                .OrderBy(x => x.Name).Asc
                .Fetch(SelectMode.Fetch, x => x.BibliographicFormat)
                .List();
        }

        public IList<BibliographicFormat> GetAllBibliographicFormats()
        {
            return GetSession().QueryOver<BibliographicFormat>()
                .OrderBy(x => x.Name).Asc
                .List();
        }
    }
}