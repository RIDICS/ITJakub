using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.OaiPmh;
using Vokabular.ProjectImport.Works.ExternalRepositoryManagement;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Managers
{
    public class ExternalRepositoryManager
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly ImportHistoryRepository m_importHistoryRepository;
        private readonly CommunicationFactory m_communicationFactory;

        public ExternalRepositoryManager(ExternalRepositoryRepository externalRepositoryRepository, ImportHistoryRepository importHistoryRepository,
            CommunicationFactory communicationFactory)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_importHistoryRepository = importHistoryRepository;
            m_communicationFactory = communicationFactory;
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

        public PagedResultList<ExternalRepositoryContract> GetExternalRepositoryList(int start, int count)
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetExternalRepositoryList(start, count));

            return new PagedResultList<ExternalRepositoryContract>
            {
                List = Mapper.Map<List<ExternalRepositoryContract>>(result.List),
                TotalCount = result.Count,
            };
        }

        public IList<ExternalRepositoryContract> GetAllExternalRepositories()
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetAllExternalRepositories());
            return Mapper.Map<IList<ExternalRepositoryContract>>(result);
        }

        public ExternalRepositoryStatisticsContract GetExternalRepositoryStatistics(int externalRepositoryId)
        {
            var work = new GetExternalRepositoryStatisticsWork(m_externalRepositoryRepository, m_importHistoryRepository,externalRepositoryId);
            work.Execute();

            
            DateTime? lastUpdateDate = null;
            bool? lastUpdateIsSuccessful = null;
            UserContract updatedByUser = null;

            if (work.LastImportHistory != null)
            {
                updatedByUser = work.LastImportHistory.CreatedByUser == null ? null : Mapper.Map<UserContract>(work.LastImportHistory.CreatedByUser);
                lastUpdateDate = work.LastImportHistory.Date;
                lastUpdateIsSuccessful = work.LastImportHistory.Status != ImportStatusEnum.Failed;
            }

            return new ExternalRepositoryStatisticsContract
            {
                TotalImportedItems = work.TotalImportStatistics.NewItems,
                TotalItemsInLastUpdate = work.LastImportStatisticsResult.TotalItems,
                NewItemsInLastUpdate = work.LastImportStatisticsResult.NewItems,
                UpdatedItemsInLastUpdate = work.LastImportStatisticsResult.UpdatedItems,
                UpdatedBy = updatedByUser,
                LastUpdateDate = lastUpdateDate,
                IsSuccessful = lastUpdateIsSuccessful
            };
        }

        public IList<ExternalRepositoryTypeContract> GetAllExternalRepositoryTypes()
        {
            var result = m_externalRepositoryRepository.InvokeUnitOfWork(x => x.GetAllExternalRepositoryTypes());
            return Mapper.Map<IList<ExternalRepositoryTypeContract>>(result);
        }

        public async Task<OaiPmhRepositoryInfoContract> GetOaiPmhRepositoryInfo(string url)
        {
            using (var client = m_communicationFactory.CreateOaiPmhCommunicationClient(url))
            {
                var result = await client.GetRepositoryInfoAsync();
                return Mapper.Map<OaiPmhRepositoryInfoContract>(result);
            }
        }
    }
}