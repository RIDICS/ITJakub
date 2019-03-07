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
        private readonly ExternalRepository m_externalRepository;
        private readonly int m_userId;

        public CreateImportHistoryWork(ImportHistoryRepository importHistoryRepository, ExternalRepository externalRepository, int userId) : base(importHistoryRepository)
        {
            m_importHistoryRepository = importHistoryRepository;
            m_externalRepository = externalRepository;
            m_userId = userId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_importHistoryRepository.Load<User>(m_userId);

            var importHistory = new ImportHistory
            {
                Date = now,
                ExternalRepository = m_externalRepository,
                Status = ImportStatusEnum.Running,
                CreatedByUser = user
            };

            return (int)m_importHistoryRepository.Create(importHistory);
        }
    }
}
