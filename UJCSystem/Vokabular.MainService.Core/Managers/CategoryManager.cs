using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.CategoryManagement;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class CategoryManager
    {
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumSiteManager m_forumSiteManager;

        public CategoryManager(CategoryRepository categoryRepository, ForumSiteManager forumSiteManager)
        {
            m_categoryRepository = categoryRepository;
            m_forumSiteManager = forumSiteManager;
        }

        public int CreateCategory(CategoryContract category)
        {
            var resultId = new CreateCategoryWork(m_categoryRepository, category).Execute();
            m_forumSiteManager.CreateCategory(category, resultId);
            return resultId;
        }

        public List<CategoryContract> GetCategoryList()
        {
            var result = m_categoryRepository.InvokeUnitOfWork(x => x.GetCategoryList());
            return Mapper.Map<List<CategoryContract>>(result);
        }

        public void DeleteCategory(int categoryId)
        {
            var deleteCategoryWork = new DeleteCategoryWork(m_categoryRepository, categoryId);
            m_forumSiteManager.DeleteCategory(categoryId); 
            deleteCategoryWork.Execute();
        }

        public void UpdateCategory(int categoryId, CategoryContract category)
        {
            var updateCategoryWork = new UpdateCategoryWork(m_categoryRepository, categoryId, category);
            m_forumSiteManager.UpdateCategory(category, GetCategory(categoryId));
            updateCategoryWork.Execute();
        }

        public CategoryContract GetCategory(int categoryId)
        {
            var result = m_categoryRepository.InvokeUnitOfWork(x => x.FindById<Category>(categoryId));
            return Mapper.Map<CategoryContract>(result);
        }

        public List<CategoryTreeContract> GetCategoryTree()
        {
            var result = m_categoryRepository.InvokeUnitOfWork(x => x.GetCategoriesWithSubcategories());
            return Mapper.Map<List<CategoryTreeContract>>(result);
        }
    }
}