using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class CreateOrUpdateTermCategoryWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int? m_termCategoryId;
        private readonly TermCategoryContract m_data;

        public CreateOrUpdateTermCategoryWork(CatalogValueRepository catalogValueRepository, int? termCategoryId, TermCategoryContract data) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_termCategoryId = termCategoryId;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var termCategory = m_termCategoryId != null
                ? m_catalogValueRepository.FindById<TermCategory>(m_termCategoryId.Value)
                : new TermCategory();

            if (termCategory == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            termCategory.Name = m_data.Name;
            
            m_catalogValueRepository.Save(termCategory);

            return termCategory.Id;
        }
    }
}