using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext;
using ITJakub.FileProcessing.Core.Sessions.Works;
using ITJakub.FileProcessing.DataContracts;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly IFulltextResourceProcessor m_fulltextResourceProcessor;
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly PersonRepository m_personRepository;
        private readonly PermissionRepository m_permissionRepository;

        public RelationalDbStoreProcessor(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            ResourceRepository resourceRepository, IFulltextResourceProcessor fulltextResourceProcessor, CatalogValueRepository catalogValueRepository, PersonRepository personRepository, 
            PermissionRepository permissionRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_resourceRepository = resourceRepository;
            m_fulltextResourceProcessor = fulltextResourceProcessor;
            m_catalogValueRepository = catalogValueRepository;
            m_personRepository = personRepository;
            m_permissionRepository = permissionRepository;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var autoImportPermissions = resourceDirector.GetSessionInfoValue<IList<PermissionFromAuthContract>>(SessionInfo.AutoImportPermissions);
            var bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            bookData.FileNameMapping = new Dictionary<string, FileResource>();
            foreach (var fileResource in resourceDirector.Resources.Where(x => x.NewNameInStorage != null))
            {
                bookData.FileNameMapping.Add(fileResource.FileName, fileResource);
            }

            bookData.ContainsEditionNote = true; //HACK always create new EditionNoteResource, TODO determine if edition note exists

            var saveNewBookDataWork = new SaveNewBookDataWork(m_projectRepository, m_metadataRepository, m_resourceRepository, m_catalogValueRepository, m_personRepository, resourceDirector);
            saveNewBookDataWork.Execute();

            var projectId = saveNewBookDataWork.ProjectId;
            var userId = saveNewBookDataWork.UserId;
            var message = saveNewBookDataWork.Message;
            var resourceVersionIds = saveNewBookDataWork.ImportedResourceVersionIds;
            var bookVersionId = saveNewBookDataWork.BookVersionId;

            var createNewSnapshot = new CreateSnapshotForImportedDataWork(m_projectRepository, projectId, userId, resourceVersionIds, bookData, message, bookVersionId);
            createNewSnapshot.Execute();

            PublishSnapshotToExternalDatabase(createNewSnapshot.SnapshotId, projectId, bookData.Pages);

            //var bookVersionId = m_bookVersionRepository.Create(bookData);
            //var bookVersion = m_bookVersionRepository.FindById<BookVersion>(bookVersionId);

            //var categories = m_categoryRepository.GetAllCategories();
            //var categoriesDictionary = categories.ToDictionary(category => category.Id);

            //var bookVersionCategories = m_categoryRepository.GetDirectCategoriesByBookVersionId(bookVersionId);

            //var allBookVersionCategoryIds = new List<int>();
            //foreach (var bookVersionCategory in bookVersionCategories)
            //{
            //    Category category;
            //    if (categoriesDictionary.TryGetValue(bookVersionCategory.Id, out category))
            //    {
            //        allBookVersionCategoryIds.Add(category.Id);
            //        while (category.ParentCategory != null && categoriesDictionary.TryGetValue(category.ParentCategory.Id, out category))
            //        {
            //            allBookVersionCategoryIds.Add(category.Id);
            //        }
            //    }
            //}

            var processAutoImportPermission = new ProcessAutoImportPermissionWork(m_permissionRepository, projectId, createNewSnapshot.BookTypes, autoImportPermissions);
            processAutoImportPermission.Execute();
            
            //var specialPermissions = m_permissionRepository.GetAutoimportPermissionsByCategoryIdList(allBookVersionCategoryIds);
            //var groupsWithAutoimport = m_permissionRepository.GetGroupsBySpecialPermissionIds(specialPermissions.Select(x => x.Id));

            //var newPermissions = groupsWithAutoimport.Select(groupWithAutoimport => new Permission
            //{
            //    Book = bookVersion.Book,
            //    Group = groupWithAutoimport
            //});

            //foreach (var newPermission in newPermissions)
            //{
            //     m_permissionRepository.CreatePermissionIfNotExist(newPermission);
            //}
            resourceDirector.SetSessionInfoValue(SessionInfo.ProjectId, projectId);
        }

        private void PublishSnapshotToExternalDatabase(long snapshotId, long projectId, List<BookPageData> bookDataPages)
        {
            var externalIds = bookDataPages.Select(x => x.XmlId)
                .Where(x => x != null) // the page doesn't exist in fulltext database when ID is null
                .ToList();
            var metadata = m_metadataRepository.InvokeUnitOfWork(x => x.GetLatestMetadataResource(projectId));

            if (externalIds.Count > 0)
            {
                m_fulltextResourceProcessor.PublishSnapshot(snapshotId, projectId, externalIds, metadata);
            }
            else
            {
                if (m_log.IsInfoEnabled)
                {
                    m_log.Info($"Snapshot is not published to external database because the project doesn't contain any text pages. ProjectID={projectId}, SnapshotID={snapshotId}");
                }
            }
        }
    }
}