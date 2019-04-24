using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectParsing.Model.Entities;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportedRecordMetadataWork : UnitOfWorkBase<int>
    {
        private readonly ImportedRecordMetadataRepository m_importedRecordMetadataRepository;
        private readonly ImportedRecord m_data;
        private readonly int m_importHistoryId;

        public CreateImportedRecordMetadataWork(ImportedRecordMetadataRepository importedRecordMetadataRepository, ImportedRecord data, int importHistoryId) : base(importedRecordMetadataRepository)
        {
            m_importedRecordMetadataRepository = importedRecordMetadataRepository;
            m_data = data;
            m_importHistoryId = importHistoryId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var importHistory = m_importedRecordMetadataRepository.Load<ImportHistory>(m_importHistoryId);
            Project project = null;          
            ImportedProjectMetadata importedProjectMetadata = null;

            if (!(m_data.IsNew && m_data.IsFailed) && m_data.ImportedProjectMetadataId.HasValue)
            {
                importedProjectMetadata = m_importedRecordMetadataRepository.Load<ImportedProjectMetadata>(m_data.ImportedProjectMetadataId.Value);
                project = m_importedRecordMetadataRepository.Load<Project>(m_data.ProjectId);
            }

            var importMetadata = new ImportedRecordMetadata
            {
               LastUpdate = importHistory,
               LastUpdateMessage = m_data.FaultedMessage,
               Snapshot = project?.LatestPublishedSnapshot,
               ImportedProjectMetadata = importedProjectMetadata
            };

           return (int)m_importedRecordMetadataRepository.Create(importMetadata);
        }
    }
}
