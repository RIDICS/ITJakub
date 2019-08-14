using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Works.ExternalRepositoryManagement
{
    public class GetExternalRepositoryStatisticsWork : UnitOfWorkBase
    {
        private readonly ExternalRepositoryRepository m_externalRepositoryRepository;
        private readonly ImportHistoryRepository m_importHistoryRepository;
        private readonly int m_externalRepositoryId;

        public GetExternalRepositoryStatisticsWork(ExternalRepositoryRepository externalRepositoryRepository,
            ImportHistoryRepository importHistoryRepository, int externalRepositoryId) : base(externalRepositoryRepository)
        {
            m_externalRepositoryRepository = externalRepositoryRepository;
            m_importHistoryRepository = importHistoryRepository;
            m_externalRepositoryId = externalRepositoryId;
        }

        public TotalImportStatistics TotalImportStatistics { get; private set; }

        public LastImportStatisticsResult LastImportStatisticsResult { get; private set; }
        
        public ImportHistory LastImportHistory { get; private set; }

        protected override void ExecuteWorkImplementation()
        {
            var externalRepository = m_externalRepositoryRepository.FindById<ExternalRepository>(m_externalRepositoryId);
            if (externalRepository == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            TotalImportStatistics = m_externalRepositoryRepository.GetExternalRepositoryStatistics(m_externalRepositoryId);
            LastImportStatisticsResult = m_externalRepositoryRepository.GetLastUpdateExternalRepositoryStatistics(m_externalRepositoryId);
            LastImportHistory = m_importHistoryRepository.GetLastImportHistory(m_externalRepositoryId);
        }
    }
}