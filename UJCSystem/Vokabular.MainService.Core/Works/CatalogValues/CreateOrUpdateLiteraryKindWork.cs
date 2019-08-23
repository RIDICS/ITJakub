using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

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
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");

            literaryKind.Name = m_name;

            m_catalogValueRepository.Save(literaryKind);

            return literaryKind.Id;
        }
    }
}