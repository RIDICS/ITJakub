using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.Category;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Managers
{
    public class CategoryManager
    {
        private readonly CategoryRepository m_categoryRepository;

        public CategoryManager(CategoryRepository categoryRepository)
        {
            m_categoryRepository = categoryRepository;
        }

        public int CreateCategory(CategoryContract category)
        {
            var resultId = new CreateCategoryWork(m_categoryRepository, category).Execute();
            return resultId;
        }

        public List<CategoryContract> GetCategoryList()
        {
            var result = new GetCategoryListWork(m_categoryRepository).Execute();
            return Mapper.Map<List<CategoryContract>>(result);
        }
    }
}