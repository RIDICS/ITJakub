using System.Net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CategoryManagement
{
    public class DeleteCategoryWork : UnitOfWorkBase
    {
        private readonly CategoryRepository m_categoryRepository;
        private readonly int m_categoryId;

        public DeleteCategoryWork(CategoryRepository categoryRepository, int categoryId) : base(categoryRepository)
        {
            m_categoryRepository = categoryRepository;
            m_categoryId = categoryId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var category = m_categoryRepository.GetCategoryWithSubcategories(m_categoryId);

            if (category == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            if (category.Categories.Count > 0)
                throw new HttpErrorCodeException("Category has some sub-categories", HttpStatusCode.BadRequest);

            m_categoryRepository.Delete(category);
        }
    }
}