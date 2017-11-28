using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class CreateOrUpdateLiteraryKindWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int? m_kindId;
        private readonly string m_name;

        public CreateOrUpdateLiteraryKindWork(CatalogValueRepository catalogValueRepository, int? kindId, string name) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_kindId = kindId;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryKind = m_kindId != null
                ? m_catalogValueRepository.FindById<LiteraryKind>(m_kindId.Value)
                : new LiteraryKind();

            if (literaryKind == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            literaryKind.Name = m_name;

            m_catalogValueRepository.Save(literaryKind);

            return literaryKind.Id;
        }
    }
}