using System.Reflection;
using ITJakub.FileProcessing.Core.Data;
using log4net;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ProjectRepository m_projectRepository;

        public RelationalDbStoreProcessor(ProjectRepository projectRepository)
        {
            m_projectRepository = projectRepository;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var bookData = resourceDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);

            // TODO save all new project data to database

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