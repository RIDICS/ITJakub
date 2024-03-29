using System.Linq;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CategoryManagement
{
    public class UpdateCategoryWork : UnitOfWorkBase
    {
        private readonly CategoryRepository m_categoryRepository;
        private readonly int m_categoryId;
        private readonly CategoryContract m_data;

        public UpdateCategoryWork(CategoryRepository categoryRepository, int categoryId, CategoryContract data) : base(categoryRepository)
        {
            m_categoryRepository = categoryRepository;
            m_categoryId = categoryId;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var category = m_categoryRepository.FindById<Category>(m_categoryId);
            if (category == null)
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");

            category.Description = m_data.Description;
            category.ExternalId = m_data.ExternalId;

            if (category.ParentCategory?.Id != m_data.ParentCategoryId)
            {
                var parentCategory = m_data.ParentCategoryId != null ? m_categoryRepository.FindById<Category>(m_data.ParentCategoryId) : null;
                var allSubcategories = m_categoryRepository.GetCategoriesByPath(category.Path);

                // Check if circle not exists
                if (parentCategory != null)
                {
                    var pathIds = parentCategory.Path.Split('/').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                    if (pathIds.Contains(category.Id))
                        throw new MainServiceException(MainServiceErrorCode.CannotMoveCategoryToSubcategory, "Can't move category into it's own subcategories", HttpStatusCode.Conflict);
                }

                // Update Path property

                var originalPath = category.Path;
                var newPath = $"{parentCategory?.Path}/{category.Id}";

                category.Path = newPath;
                category.ParentCategory = parentCategory;

                // Update Path in all subcategories

                originalPath = $"{originalPath}/";
                newPath = $"{newPath}/";
                foreach (var subcategory in allSubcategories.Where(x => x.Id != category.Id))
                {
                    subcategory.Path = subcategory.Path.Replace(originalPath, newPath);
                    m_categoryRepository.Update(subcategory);
                }
            }

            m_categoryRepository.Update(category);
        }
    }
}