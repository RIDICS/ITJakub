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

        public ProjectManager(ProjectRepository projectRepository, CatalogValueRepository catalogValueRepository,
            PersonRepository personRepository, MetadataRepository metadataRepository)
        {
            m_projectRepository = projectRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_metadataRepository = metadataRepository;
        }

        public void SaveImportedProject(ProjectImportMetadata projectImportMetadata, int userId)
        {
            if (projectImportMetadata.IsNew)
            {
                var projectId = CreateProject(projectImportMetadata, userId);
                projectImportMetadata.ProjectId = projectId;
            }

            CreateProjectMetadata(projectImportMetadata, userId);
        }

        private long CreateProject(ProjectImportMetadata projectImportMetadata, int userId)
        {
            return new CreateImportedProjectWork(m_projectRepository, projectImportMetadata, userId).Execute();
        }

        private void CreateProjectMetadata(ProjectImportMetadata projectImportMetadata, int userId)
        {
            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, projectImportMetadata, userId).Execute();

            new CreateSnapshotForImportedMetadataWork(m_projectRepository, projectImportMetadata.ProjectId, userId).Execute();
        }
    }
}