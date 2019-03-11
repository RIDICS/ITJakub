using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.ProjectImport.Managers
{
    public class FilteringExpressionSetManager
    {
        private readonly FilteringExpressionSetRepository m_filteringExpressionSetRepository;

        public FilteringExpressionSetManager(FilteringExpressionSetRepository filteringExpressionSetRepository)
        {
            m_filteringExpressionSetRepository = filteringExpressionSetRepository;
        }

        public IDictionary<string, List<string>> GetFilteringExpressionsByExternalRepository(int externalRepositoryId)
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionsByExternalRepository(externalRepositoryId));
            return result.GroupBy(expr => expr.Field).ToDictionary(group => group.Key, group => group.Select(expr => expr.Value).ToList());
        }
    }
}
