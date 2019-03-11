using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using ProjectImportMetadata = Vokabular.ProjectParsing.Model.Entities.ProjectImportMetadata;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportMetadataWork : UnitOfWorkBase<int>
    {
        private readonly ImportMetadataRepository m_importMetadataRepository;
        private readonly ProjectImportMetadata m_metadata;
        private readonly ImportHistory m_importHistory;

        public CreateImportMetadataWork(ImportMetadataRepository importMetadataRepository, ProjectImportMetadata metadata, ImportHistory importHistory) : base(importMetadataRepository)
        {
            m_importMetadataRepository = importMetadataRepository;
            m_metadata = metadata;
            m_importHistory = importHistory;
        }

        protected override int ExecuteWorkImplementation()
        {
            var project = m_importMetadataRepository.Load<Project>(m_metadata.ProjectId);

            var importMetadata = new ImportMetadata
            {
               ExternalId = m_metadata.ExternalId,
               LastUpdate = m_importHistory,
               LastUpdateMessage = m_metadata.FaultedMessage,
               Snapshot = project.LatestPublishedSnapshot
            };

           return (int)m_importMetadataRepository.Create(importMetadata);
        }
    }
}
