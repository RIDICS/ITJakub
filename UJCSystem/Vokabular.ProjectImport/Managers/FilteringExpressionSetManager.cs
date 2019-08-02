using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Works.FilteringExpressionSetManagement;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Managers
{
    public class FilteringExpressionSetManager
    {
        private readonly FilteringExpressionSetRepository m_filteringExpressionSetRepository;

        public FilteringExpressionSetManager(FilteringExpressionSetRepository filteringExpressionSetRepository)
        {
            m_filteringExpressionSetRepository = filteringExpressionSetRepository;
        }

        public int CreateFilteringExpressionSet(FilteringExpressionSetDetailContract data, int userId)
        {
            var result = new CreateFilteringExpressionSetWork(m_filteringExpressionSetRepository, data, userId).Execute();
            return result;
        }

        public void UpdateFilteringExpressionSet(int filteringExpressionSetId, FilteringExpressionSetDetailContract data)
        {
            new UpdateFilteringExpressionSetWork(m_filteringExpressionSetRepository, data, filteringExpressionSetId).Execute();
        }

        public void DeleteFilteringExpressionSet(int filteringExpressionSetId)
        {
            new DeleteFilteringExpressionSetWork(m_filteringExpressionSetRepository, filteringExpressionSetId).Execute();
        }

        public IDictionary<string, List<string>> GetFilteringExpressionsByExternalRepository(int externalRepositoryId)
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionsByExternalRepository(externalRepositoryId));
            return result.GroupBy(expr => expr.Field).ToDictionary(group => group.Key, group => group.Select(expr => expr.Value).ToList());
        }

        public FilteringExpressionSetDetailContract GetFilteringExpressionSet(int externalRepositoryId)
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionSet(externalRepositoryId));
            return Mapper.Map<FilteringExpressionSetDetailContract>(result);
        }

        public PagedResultList<FilteringExpressionSetContract> GetFilteringExpressionSetList(int startValue, int countValue)
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionSetList(startValue, countValue));

            return new PagedResultList<FilteringExpressionSetContract>
            {
                List = Mapper.Map<List<FilteringExpressionSetContract>>(result.List),
                TotalCount = result.Count,
            };
        }

        public IList<BibliographicFormatContract> GetAllBibliographicFormats()
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetAllBibliographicFormats());
            return Mapper.Map<IList<BibliographicFormatContract>>(result);
        }

        public IList<FilteringExpressionSetContract> GetAllFilteringExpressionSets()
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetAllFilteringExpressionSets());
            return Mapper.Map<IList<FilteringExpressionSetContract>>(result);
        }
    }
}
