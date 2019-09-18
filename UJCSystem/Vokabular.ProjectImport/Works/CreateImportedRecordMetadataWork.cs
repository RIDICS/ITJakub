using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Repositories.BibliographyImport;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportedRecordMetadataWork : UnitOfWorkBase<int>
    {
        private readonly ImportedRecordMetadataRepository m_importedRecordMetadataRepository;
        private readonly ProjectRepository m_projectRepository;
        private readonly ImportedRecord m_data;
        private readonly int m_importHistoryId;

        public CreateImportedRecordMetadataWork(ImportedRecordMetadataRepository importedRecordMetadataRepository, ProjectRepository projectRepository, ImportedRecord data, int importHistoryId) : base(importedRecordMetadataRepository)
        {
            m_importedRecordMetadataRepository = importedRecordMetadataRepository;
            m_projectRepository = projectRepository;
            m_data = data;
            m_importHistoryId = importHistoryId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var importHistory = m_importedRecordMetadataRepository.Load<ImportHistory>(m_importHistoryId);        
            ImportedProjectMetadata importedProjectMetadata = null;
            Snapshot latestSnapshot = null;

            if (m_data.ImportedProjectMetadataId.HasValue)
            {
                importedProjectMetadata = m_importedRecordMetadataRepository.Load<ImportedProjectMetadata>(m_data.ImportedProjectMetadataId.Value);   
            }

            if (!m_data.IsFailed)
            {
                latestSnapshot = m_projectRepository.GetLatestSnapshot(m_data.ProjectId);
            }

            var importMetadata = new ImportedRecordMetadata
            {
               LastUpdate = importHistory,
               LastUpdateMessage = m_data.FaultedMessage,
               Snapshot = latestSnapshot,
               ImportedProjectMetadata = importedProjectMetadata
            };

           return (int)m_importedRecordMetadataRepository.Create(importMetadata);
        }
    }
}
