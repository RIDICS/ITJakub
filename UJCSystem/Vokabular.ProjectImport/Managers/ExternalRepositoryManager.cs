using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.ProjectImport.Works.ExternalRepositoryManagement;
using Vokabular.RestClient.Results;

namespace Vokabular.ProjectImport.Managers
{
    public class ExternalRepositoryManager
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;

        public ExternalRepositoryManager(ExternalRepositoryRepository externalRepositoryRepository)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
        }

        public int CreateExternalRepository(ExternalRepositoryDetailContract externalRepository, int userId)
        {
            var result = new CreateExternalRepositoryWork(m_externalRepositoryRepository, externalRepository, userId).Execute();
            return result;
        }

        public ExternalRepositoryDetailContract GetExternalRepository(int externalRepositoryId)
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetExternalRepository(externalRepositoryId));
            return Mapper.Map<ExternalRepositoryDetailContract>(result);
        }

        public void UpdateExternalRepository(int externalRepositoryId, ExternalRepositoryDetailContract externalRepositoryDetailContract)
        {
            new UpdateExternalRepositoryWork(m_externalRepositoryRepository, externalRepositoryId, externalRepositoryDetailContract).Execute();
        }

        public void DeleteExternalRepository(int externalRepositoryId)
        {
            new DeleteExternalRepositoryWork(m_externalRepositoryRepository, externalRepositoryId).Execute();
        }

        public PagedResultList<ExternalRepositoryDetailContract> GetExternalRepositoryList(int start, int count)
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x =>  x.GetExternalRepositoryList(start, count));

            return new PagedResultList<ExternalRepositoryDetailContract>
            {
                List = Mapper.Map<List<ExternalRepositoryDetailContract>>(result.List),
                TotalCount = result.Count,
            };
        }

        public IList<TotalImportStatistics> GetExternalRepositoryStatisticsList()
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x =>  x.GetExternalRepositoryStatisticsList());
            return result;
        }
    }
}
