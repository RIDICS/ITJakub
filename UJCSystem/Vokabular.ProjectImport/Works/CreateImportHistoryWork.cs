using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportHistoryWork : UnitOfWorkBase<int>
    {
        private readonly ImportHistoryRepository m_importHistoryRepository;
        private readonly ExternalResource m_externalResource;
        private readonly int m_userId;

        public CreateImportHistoryWork(ImportHistoryRepository importHistoryRepository, ExternalResource externalResource, int userId) : base(importHistoryRepository)
        {
            m_importHistoryRepository = importHistoryRepository;
            m_externalResource = externalResource;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_importHistoryRepository.Load<User>(m_userId);

            var importHistory = new ImportHistory
            {
                Date = now,
                ExternalResource = m_externalResource,
                Status = ImportStatusEnum.Running,
                UpdatedByUser = user
            };

            return (int)m_importHistoryRepository.Create(importHistory);
        }
    }
}
