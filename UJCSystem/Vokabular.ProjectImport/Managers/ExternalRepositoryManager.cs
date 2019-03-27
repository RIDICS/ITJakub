using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Works.ExternalRepositoryManagement;
using Vokabular.RestClient.Results;

namespace Vokabular.ProjectImport.Managers
{
    public class ExternalRepositoryManager
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ExternalRepositoryManager(ExternalRepositoryRepository externalRepositoryRepository, AuthenticationManager authenticationManager, AuthorizationManager authorizationManager)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_authenticationManager = authenticationManager;
            m_authorizationManager = authorizationManager;
        }

        public int CreateExternalRepository(ExternalRepositoryDetailContract externalRepository)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var userId = m_authenticationManager.GetCurrentUserId();

            var result = new CreateExternalRepositoryWork(m_externalRepositoryRepository, externalRepository, userId).Execute();
            return result;
        }

        public ExternalRepositoryDetailContract GetExternalRepository(int externalRepositoryId)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetExternalRepository(externalRepositoryId));
            return Mapper.Map<ExternalRepositoryDetailContract>(result);
        }

        public void UpdateExternalRepository(int externalRepositoryId, ExternalRepositoryDetailContract externalRepositoryDetailContract)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            new UpdateExternalRepositoryWork(m_externalRepositoryRepository, externalRepositoryId, externalRepositoryDetailContract).Execute();
        }

        public void DeleteExternalRepository(int externalRepositoryId)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();
            new DeleteExternalRepositoryWork(m_externalRepositoryRepository, externalRepositoryId).Execute();
        }

        public PagedResultList<ExternalRepositoryDetailContract> GetExternalRepositoryList(int? start, int? count)
        {
            m_authorizationManager.CheckUserCanManageRepositoryImport();

            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x =>  x.GetExternalRepositoryList(startValue, countValue));

            return new PagedResultList<ExternalRepositoryDetailContract>
            {
                List = Mapper.Map<List<ExternalRepositoryDetailContract>>(result.List),
                TotalCount = result.Count,
            };
        }
    }
}
