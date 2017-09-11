using System.Collections.Generic;
using System.Reflection;
using ITJakub.FileProcessing.Core.Sessions.Works;
using log4net;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ProjectRepository m_projectRepository;
        private readonly MetadataRepository m_metadataRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ResourceRepository m_resourceRepository;

        public RelationalDbStoreProcessor(ProjectRepository projectRepository, MetadataRepository metadataRepository, CategoryRepository categoryRepository,
            ResourceRepository resourceRepository)
        {
            m_projectRepository = projectRepository;
            m_metadataRepository = metadataRepository;
            m_categoryRepository = categoryRepository;
            m_resourceRepository = resourceRepository;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var saveNewBookDataWork = new SaveNewBookDataWork(m_projectRepository, m_metadataRepository, m_resourceRepository, resourceDirector);
            saveNewBookDataWork.Execute();

            // TODO determine if Snapshot should be created
            var projectId = saveNewBookDataWork.ProjectId;
            var userId = saveNewBookDataWork.UserId;
            var message = saveNewBookDataWork.Message;
            var resourceVersionIds = saveNewBookDataWork.ImportedResourceVersionIds;
            var bookData = saveNewBookDataWork.BookData;

            var createNewSnapshot = new CreateSnapshotForImportedDataWork(m_projectRepository, projectId, userId, resourceVersionIds, bookData, message);
            createNewSnapshot.Execute();

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
    }
}