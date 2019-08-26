using NHibernate.Exceptions;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class DeleteTermWork : UnitOfWorkBase
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int m_termId;

        public DeleteTermWork(CatalogValueRepository catalogValueRepository, int termId) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_termId = termId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var item = m_catalogValueRepository.FindById<Term>(m_termId);

            if (item == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            try
            {
                m_catalogValueRepository.Delete(item);
                m_catalogValueRepository.UnitOfWork.CurrentSession.Flush();
            }
            catch (GenericADOException)
            {
                throw new MainServiceException(MainServiceErrorCode.DeleteResourcePageRelation, "Could not delete resource. Existing relation to resource Page?");
            }
        }
    }
}