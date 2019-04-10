using System.Collections.Generic;
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
        private readonly PermissionRepository m_permissionRepository;

        public ProjectManager(ProjectRepository projectRepository, CatalogValueRepository catalogValueRepository,
            PersonRepository personRepository, MetadataRepository metadataRepository, PermissionRepository permissionRepository)
        {
            m_projectRepository = projectRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_metadataRepository = metadataRepository;
            m_permissionRepository = permissionRepository;
        }

        public void SaveImportedProject(ImportedRecord importedRecord, int userId, int externalRepositoryId, int bookTypeId, IList<int> groupsWithPermissionIds)
        {
            new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                m_personRepository, m_permissionRepository, importedRecord, userId, externalRepositoryId, bookTypeId, groupsWithPermissionIds).Execute();
        }
    }
}