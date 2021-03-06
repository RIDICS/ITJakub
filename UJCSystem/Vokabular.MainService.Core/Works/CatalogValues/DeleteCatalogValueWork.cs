﻿using NHibernate.Exceptions;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

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
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            try
            {
                m_catalogValueRepository.Delete(item);
                m_catalogValueRepository.UnitOfWork.CurrentSession.Flush();
            }
            catch (GenericADOException)
            {
                throw new MainServiceException(MainServiceErrorCode.DeleteResourceProjectRelation, "Could not delete resource. Existing relation to Project?");
            }
        }
    }
}