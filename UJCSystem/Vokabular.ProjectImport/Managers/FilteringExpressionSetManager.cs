using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Works.FilteringExpressionSetManagement;
using Vokabular.RestClient.Results;

namespace Vokabular.ProjectImport.Managers
{
    public class FilteringExpressionSetManager
    {
        private readonly FilteringExpressionSetRepository m_filteringExpressionSetRepository;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly AuthenticationManager m_authenticationManager;

        public FilteringExpressionSetManager(FilteringExpressionSetRepository filteringExpressionSetRepository, AuthorizationManager authorizationManager, AuthenticationManager authenticationManager)
        {
            m_filteringExpressionSetRepository = filteringExpressionSetRepository;
            m_authorizationManager = authorizationManager;
            m_authenticationManager = authenticationManager;
        }

        public int CreateFilteringExpressionSet(FilteringExpressionSetDetailContract data)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var userId = m_authenticationManager.GetCurrentUserId();
            var result = new CreateFilteringExpressionSetWork(m_filteringExpressionSetRepository, data, userId).Execute();
            return result;
        }

        public void UpdateFilteringExpressionSet(int filteringExpressionSetId, FilteringExpressionSetDetailContract data)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            new UpdateFilteringExpressionSetWork(m_filteringExpressionSetRepository, data, filteringExpressionSetId).Execute();
        }

        public void DeleteFilteringExpressionSet(int filteringExpressionSetId)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            new DeleteFilteringExpressionSetWork(m_filteringExpressionSetRepository, filteringExpressionSetId).Execute();
        }

        public IDictionary<string, List<string>> GetFilteringExpressionsByExternalRepository(int externalRepositoryId)
        {
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionsByExternalRepository(externalRepositoryId));
            return result.GroupBy(expr => expr.Field).ToDictionary(group => group.Key, group => group.Select(expr => expr.Value).ToList());
        }

        public FilteringExpressionSetDetailContract GetFilteringExpressionSet(int externalRepositoryId)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionSet(externalRepositoryId));
            return Mapper.Map<FilteringExpressionSetDetailContract>(result);
        }

        public PagedResultList<FilteringExpressionSetContract> GetFilteringExpressionSetList(int? start, int? count)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();

            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var result = m_filteringExpressionSetRepository.InvokeUnitOfWork(x => x.GetFilteringExpressionSetList(startValue, countValue));

            return new PagedResultList<FilteringExpressionSetContract>
            {
                List = Mapper.Map<List<FilteringExpressionSetContract>>(result.List),
                TotalCount = result.Count,
            };
        }
    }
}
