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
        private readonly IMapper m_mapper;

        public CategoryManager(CategoryRepository categoryRepository, ForumSiteManager forumSiteManager, IMapper mapper)
        {
            m_categoryRepository = categoryRepository;
            m_forumSiteManager = forumSiteManager;
            m_mapper = mapper;
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
            return m_mapper.Map<List<CategoryContract>>(result);
        }

        public void DeleteCategory(int categoryId)
        {
            var deleteCategoryWork = new DeleteCategoryWork(m_categoryRepository, categoryId);
            m_forumSiteManager.DeleteCategory(categoryId); 
            deleteCategoryWork.Execute();
        }

        public void UpdateCategory(int categoryId, CategoryContract category)
        {
            var oldCategory = GetCategory(categoryId);
            var updateCategoryWork = new UpdateCategoryWork(m_categoryRepository, categoryId, category);
            m_forumSiteManager.UpdateCategory(category, oldCategory);
            updateCategoryWork.Execute();
        }

        public CategoryContract GetCategory(int categoryId)
        {
            var result = m_categoryRepository.InvokeUnitOfWork(x => x.FindById<Category>(categoryId));
            return m_mapper.Map<CategoryContract>(result);
        }

        public List<CategoryTreeContract> GetCategoryTree()
        {
            var result = m_categoryRepository.InvokeUnitOfWork(x => x.GetCategoriesWithSubcategories());
            return m_mapper.Map<List<CategoryTreeContract>>(result);
        }
    }
}