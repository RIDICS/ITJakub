using System.Net;
using NHibernate.Exceptions;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class DeleteCatalogValueWork<T> : UnitOfWorkBase where T : ICatalogValue
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int m_itemId;

        public DeleteCatalogValueWork(CatalogValueRepository catalogValueRepository, int itemId) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_itemId = itemId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var item = m_catalogValueRepository.FindById<T>(m_itemId);

            if (item == null)
            {
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            }

            try
            {
                m_catalogValueRepository.Delete(item);
                m_catalogValueRepository.UnitOfWork.CurrentSession.Flush();
            }
            catch (GenericADOException exception)
            {
                throw new HttpErrorCodeException("Could not delete resource. Existing relation to Project?", exception, HttpStatusCode.BadRequest);
            }
        }
    }
}