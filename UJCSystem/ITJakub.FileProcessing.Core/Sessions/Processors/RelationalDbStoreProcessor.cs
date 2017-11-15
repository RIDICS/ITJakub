using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext;
using ITJakub.FileProcessing.Core.Sessions.Works;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly IFulltextResourceProcessor m_fulltextResourceProcessor;

        public RelationalDbStoreProcessor(ProjectRepository projectRepository, MetadataRepository metadataRepository,
            ResourceRepository resourceRepository, IFulltextResourceProcessor fulltextResourceProcessor)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_resourceRepository = resourceRepository;
            m_fulltextResourceProcessor = fulltextResourceProcessor;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);
            bookData.FileNameMapping = new Dictionary<string, FileResource>();
            foreach (var fileResource in resourceDirector.Resources.Where(x => x.NewNameInStorage != null))
            {
                bookData.FileNameMapping.Add(fileResource.FileName, fileResource);
            }

            var saveNewBookDataWork = new SaveNewBookDataWork(m_projectRepository, m_metadataRepository, m_resourceRepository, resourceDirector);
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
        }

        private void PublishSnapshotToExternalDatabase(long snapshotId, long projectId, List<BookPageData> bookDataPages)
        {
            var externalIds = bookDataPages.Select(x => x.XmlId).ToList();
            m_fulltextResourceProcessor.PublishSnapshot(snapshotId, projectId, externalIds);
        }
    }
}