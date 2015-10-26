using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;
        private readonly PermissionRepository m_permissionRepository;

        public RelationalDbStoreProcessor(BookVersionRepository bookVersionRepository, CategoryRepository categoryRepository, PermissionRepository permissionRepository)
        {
            m_bookVersionRepository = bookVersionRepository;
            m_categoryRepository = categoryRepository;
            m_permissionRepository = permissionRepository;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var bookEntity = resourceDirector.GetSessionInfoValue<BookVersion>(SessionInfo.BookVersionEntity);

            var bookVersionId = m_bookVersionRepository.Create(bookEntity);
            var bookVersion = m_bookVersionRepository.FindById<BookVersion>(bookVersionId);

            var categories = m_categoryRepository.GetAllCategories();
            var categoriesDictionary = categories.ToDictionary(category => category.Id);

            var bookVersionCategories = m_categoryRepository.GetDirectCategoriesByBookVersionId(bookVersionId);

            var allBookVersionCategoryIds = new List<int>();
            foreach (var bookVersionCategory in bookVersionCategories)
            {
                Category category;
                if (categoriesDictionary.TryGetValue(bookVersionCategory.Id, out category))
                {
                    allBookVersionCategoryIds.Add(category.Id);
                    while (category.ParentCategory != null && categoriesDictionary.TryGetValue(category.ParentCategory.Id, out category))
                    {
                        allBookVersionCategoryIds.Add(category.Id);
                    }
                }
            }

            var specialPermissions = m_permissionRepository.GetAutoimportPermissionsByCategoryIdList(allBookVersionCategoryIds);
            var groupsWithAutoimport = m_permissionRepository.GetGroupsBySpecialPermissionIds(specialPermissions.Select(x => x.Id));

            var newPermissions = groupsWithAutoimport.Select(groupWithAutoimport => new Permission
            {
                Book = bookVersion.Book,
                Group = groupWithAutoimport
            });

            foreach (var newPermission in newPermissions)
            {
                try
                {
                    m_permissionRepository.Create(newPermission);
                }
                catch (Exception)
                {
                    if (m_log.IsInfoEnabled)
                    {
                        m_log.Info(string.Format("Group with id '{0}' already have permission on book with '{1}'",
                            newPermission.Group.Id, newPermission.Book.Id));
                    }
                }
            }
        }
    }
}