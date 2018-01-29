using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateCategoriesSubtask
    {
        private readonly CategoryRepository m_categoryRepository;

        public UpdateCategoriesSubtask(CategoryRepository categoryRepository)
        {
            m_categoryRepository = categoryRepository;
        }

        public void UpdateCategoryList(long projectId, BookData bookData)
        {
            var dbCategories = m_categoryRepository.GetCategoryList();
            
            foreach (var categoryData in bookData.AllCategoriesHierarchy)
            {
                UpdateCategory(dbCategories, null, categoryData);
            }

            // Assign only new categories
            var project = m_categoryRepository.GetProjectWithCategories(projectId);
            foreach (var categoryXmlId in bookData.CategoryXmlIds)
            {
                if (project.Categories.Any(x => x.ExternalId == categoryXmlId))
                {
                    continue;
                }

                var category = dbCategories.First(x => x.ExternalId == categoryXmlId);
                project.Categories.Add(category);
            }
            m_categoryRepository.Update(project);
        }

        private void UpdateCategory(IList<Category> dbCategories, Category parentCategory, CategoryData categoryData)
        {
            var parentPath = parentCategory != null ? parentCategory.Path : string.Empty;
            var dbCategory = dbCategories.FirstOrDefault(x => x.ExternalId == categoryData.XmlId);
            if (dbCategory == null)
            {
                dbCategory = new Category
                {
                    Categories = new List<Category>(),
                    Description = categoryData.Description,
                    ExternalId = categoryData.XmlId,
                    Path = string.Empty,
                    ParentCategory = parentCategory
                };
                var categoryId = m_categoryRepository.Create(dbCategory);

                dbCategory.Path = $"{parentPath}/{categoryId}";
                m_categoryRepository.Update(dbCategory);

                dbCategories.Add(dbCategory);
            }

            foreach (var subcategoryData in categoryData.SubCategories)
            {
                UpdateCategory(dbCategories, dbCategory, subcategoryData);
            }
        }
    }
}