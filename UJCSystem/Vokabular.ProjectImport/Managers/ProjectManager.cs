using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Managers
{
    public class ProjectManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly PersonRepository m_personRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly ImportedProjectMetadataManager m_importedProjectMetadataManager;
        private readonly PermissionRepository m_permissionRepository;

        public ProjectManager(ProjectRepository projectRepository, CatalogValueRepository catalogValueRepository,
            PersonRepository personRepository, MetadataRepository metadataRepository, ImportedProjectMetadataManager importedProjectMetadataManager, PermissionRepository permissionRepository)
        {
            m_projectRepository = projectRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_metadataRepository = metadataRepository;
            m_importedProjectMetadataManager = importedProjectMetadataManager;
            m_permissionRepository = permissionRepository;
        }

        public void SaveImportedProject(ImportedRecord importedRecord, int userId, int externalRepositoryId)
        {
            if (importedRecord.IsNew)
            {
                var projectId = CreateProject(importedRecord, userId);
                importedRecord.ProjectId = projectId;

                var id = m_importedProjectMetadataManager.CreateImportedProjectMetadata(importedRecord, externalRepositoryId);
                importedRecord.ImportedProjectMetadataId = id;
            }

            CreateProjectMetadata(importedRecord, userId);
            new ProcessExternalImportPermissionWork(m_permissionRepository, importedRecord.ProjectId).Execute();
        }

        private long CreateProject(ImportedRecord importedRecord, int userId)
        {
            var work = new CreateImportedProjectWork(m_projectRepository, importedRecord, userId);
            return work.Execute();
        }

        private void CreateProjectMetadata(ImportedRecord importedRecord, int userId)
        {
            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, importedRecord, userId).Execute();

            new CreateSnapshotForImportedMetadataWork(m_projectRepository, importedRecord.ProjectId, userId).Execute();
        }
    }
}