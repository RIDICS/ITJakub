using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.ProjectImport.Works.ExternalRepositoryManagement;
using Vokabular.RestClient.Results;

namespace Vokabular.ProjectImport.Managers
{
    public class ExternalRepositoryManager
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly CommunicationManager m_communicationManager;

        public ExternalRepositoryManager(ExternalRepositoryRepository externalRepositoryRepository,
            CommunicationManager communicationManager)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_communicationManager = communicationManager;
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
            new UpdateExternalRepositoryWork(m_externalRepositoryRepository, externalRepositoryId, externalRepositoryDetailContract)
                .Execute();
        }

        public void DeleteExternalRepository(int externalRepositoryId)
        {
            new DeleteExternalRepositoryWork(m_externalRepositoryRepository, externalRepositoryId).Execute();
        }

        public PagedResultList<ExternalRepositoryDetailContract> GetExternalRepositoryList(int start, int count)
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetExternalRepositoryList(start, count));

            return new PagedResultList<ExternalRepositoryDetailContract>
            {
                List = Mapper.Map<List<ExternalRepositoryDetailContract>>(result.List),
                TotalCount = result.Count,
            };
        }

        public ExternalRepositoryStatisticsContract GetExternalRepositoryStatistics(int externalRepositoryId)
        {
            var work = new GetExternalRepositoryStatisticsWork(m_externalRepositoryRepository, externalRepositoryId);
            work.Execute();
            return new ExternalRepositoryStatisticsContract
            {
                TotalImportedItems = work.TotalImportStatistics.NewItems,
                TotalItemsInLastUpdate = work.LastImportStatisticsResult.TotalItems,
                NewItemsInLastUpdate = work.LastImportStatisticsResult.NewItems,
                UpdatedItemsInLastUpdate = work.LastImportStatisticsResult.UpdatedItems
            };
        }

        public IList<ExternalRepositoryTypeContract> GetAllExternalRepositoryTypes()
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetAllExternalRepositoryTypes());
            return Mapper.Map<IList<ExternalRepositoryTypeContract>>(result);
        }

        public async Task<OaiPmhRepositoryInfoContract> GetOaiPmhRepositoryInfo(string url)
        {
            using (var client = m_communicationManager.GetOaiPmhCommunicationClient(url))
            {
                var result = await client.GetRepositoryInfoAsync();
                return Mapper.Map<OaiPmhRepositoryInfoContract>(result);
            }
        }
    }
}