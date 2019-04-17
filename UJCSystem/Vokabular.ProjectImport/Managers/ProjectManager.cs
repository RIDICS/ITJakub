using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.ProjectImport.Model;
using Vokabular.ProjectImport.Works;
using Vokabular.ProjectParsing.Model.Entities;
using Vokabular.Shared.Extensions;

namespace Vokabular.ProjectImport.Managers
{
    public class ProjectManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly PersonRepository m_personRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly ImportedRecordMetadataManager m_importedRecordMetadataManager;
        private ILogger<ProjectManager> m_logger;

        public ProjectManager(ProjectRepository projectRepository, CatalogValueRepository catalogValueRepository,
            PersonRepository personRepository, MetadataRepository metadataRepository, PermissionRepository permissionRepository,
            ImportedRecordMetadataManager importedRecordMetadataManager, ILogger<ProjectManager> logger)
        {
            m_projectRepository = projectRepository;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_metadataRepository = metadataRepository;
            m_permissionRepository = permissionRepository;
            m_importedRecordMetadataManager = importedRecordMetadataManager;
            m_logger = logger;
        }

        public void SaveImportedProject(ImportedRecord importedRecord, int userId, int externalRepositoryId, int bookTypeId,
            IList<int> groupsWithPermissionIds, int importHistoryId, RepositoryImportProgressInfo progressInfo)
        {
            try
            {
                if (importedRecord.IsFailed)
                {
                    progressInfo.IncrementFailedProjectsCount();
                }
                else
                {
                    new SaveImportedDataWork(m_projectRepository, m_metadataRepository, m_catalogValueRepository,
                        m_personRepository, m_permissionRepository, importedRecord, userId, externalRepositoryId, bookTypeId,
                        groupsWithPermissionIds).Execute();
                }
            }
            catch (DataException e)
            {
                importedRecord.IsFailed = true;
                importedRecord.FaultedMessage = e.Message;
                progressInfo.IncrementFailedProjectsCount();

                if (m_logger.IsErrorEnabled())
                    m_logger.LogError(e, e.Message);
            }
            finally
            {
                m_importedRecordMetadataManager.CreateImportedRecordMetadata(importedRecord, importHistoryId);
                progressInfo.IncrementProcessedProjectsCount();
            }
        }
    }
}