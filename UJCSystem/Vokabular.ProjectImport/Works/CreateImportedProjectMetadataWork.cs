using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.ProjectParsing.Model.Entities;
using Project = Vokabular.DataEntities.Database.Entities.Project;

namespace Vokabular.ProjectImport.Works
{
    public class CreateImportedProjectMetadataWork : UnitOfWorkBase<int>
    {
        private readonly ImportedProjectMetadataRepository m_importedProjectMetadataRepository;
        private readonly ImportedRecord m_metadata;
        private readonly int m_externalRepositoryId;

        public CreateImportedProjectMetadataWork(ImportedProjectMetadataRepository importedProjectMetadataRepository, ImportedRecord metadata, int externalRepositoryId) : base(importedProjectMetadataRepository)
        {
            m_importedProjectMetadataRepository = importedProjectMetadataRepository;
            m_metadata = metadata;
            m_externalRepositoryId = externalRepositoryId;
        }

        protected override int ExecuteWorkImplementation()
        {
            var project = m_importedProjectMetadataRepository.Load<Project>(m_metadata.ProjectId);
            var externalRepository = m_importedProjectMetadataRepository.Load<ExternalRepository>(m_externalRepositoryId);

            var importedProjectMetadata = new ImportedProjectMetadata
            {
               ExternalId = m_metadata.ExternalId,
               Project = project,
               ExternalRepository = externalRepository
            };

           return (int)m_importedProjectMetadataRepository.Create(importedProjectMetadata);
        }
    }
}
