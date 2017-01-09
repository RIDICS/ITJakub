using System.Collections.Generic;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Category
{
    public class GetCategoryListWork : UnitOfWorkBase<IList<DataEntities.Database.Entities.Category>>
    {
        private readonly CategoryRepository m_categoryRepository;

        public GetCategoryListWork(CategoryRepository categoryRepository) : base(categoryRepository.UnitOfWork)
        {
            m_categoryRepository = categoryRepository;
        }

        protected override IList<DataEntities.Database.Entities.Category> ExecuteWorkImplementation()
        {
            var result = m_categoryRepository.GetCategoryList();
            return result;
        }
    }
}