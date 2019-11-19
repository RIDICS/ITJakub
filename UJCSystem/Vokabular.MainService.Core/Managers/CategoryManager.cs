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
        private readonly IMapper m_mapper;

        public CategoryManager(CategoryRepository categoryRepository, IMapper mapper)
        {
            m_categoryRepository = categoryRepository;
            m_mapper = mapper;
        }

        public int CreateCategory(CategoryContract category)
        {
            var resultId = new CreateCategoryWork(m_categoryRepository, category).Execute();
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
            deleteCategoryWork.Execute();
        }

        public void UpdateCategory(int categoryId, CategoryContract category)
        {
            var updateCategoryWork = new UpdateCategoryWork(m_categoryRepository, categoryId, category);
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