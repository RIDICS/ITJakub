using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportHistoryWork : UnitOfWorkBase<int>
    {
        private readonly ImportHistoryRepository m_importHistoryRepository;
        private readonly int m_externalRepositoryId;
        private readonly int m_userId;

        public CreateImportHistoryWork(ImportHistoryRepository importHistoryRepository, int externalRepositoryId, int userId) : base(importHistoryRepository)
        {
            m_importHistoryRepository = importHistoryRepository;
            m_externalRepositoryId = externalRepositoryId;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_importHistoryRepository.Load<User>(m_userId);
            var externalRepository = m_importHistoryRepository.Load<ExternalRepository>(m_externalRepositoryId);

            var importHistory = new ImportHistory
            {
                Date = now,
                ExternalRepository = externalRepository,
                Status = ImportStatusEnum.Running,
                CreatedByUser = user
            };

            return (int)m_importHistoryRepository.Create(importHistory);
        }
    }
}
