using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class CreateOrUpdateLiteraryOriginalWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int? m_originalId;
        private readonly string m_name;

        public CreateOrUpdateLiteraryOriginalWork(CatalogValueRepository catalogValueRepository, int? originalId, string name) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_originalId = originalId;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryOriginal = m_originalId != null
                ? m_catalogValueRepository.FindById<LiteraryOriginal>(m_originalId.Value)
                : new LiteraryOriginal();

            if (literaryOriginal == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            literaryOriginal.Name = m_name;

            m_catalogValueRepository.Save(literaryOriginal);

            return literaryOriginal.Id;
        }
    }
}