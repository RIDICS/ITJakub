using System.Net;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class CreateOrUpdateResponsibleTypeWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int? m_responsibleTypeId;
        private readonly ResponsibleTypeContract m_data;

        public CreateOrUpdateResponsibleTypeWork(CatalogValueRepository catalogValueRepository, int? responsibleTypeId, ResponsibleTypeContract data) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_responsibleTypeId = responsibleTypeId;
            m_data = data;
        }

        protected override int ExecuteWorkImplementation()
        {
            var responsibleType = m_responsibleTypeId != null
                ? m_catalogValueRepository.FindById<ResponsibleType>(m_responsibleTypeId.Value)
                : new ResponsibleType();

            if (responsibleType == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            var typeEnum = Mapper.Map<ResponsibleTypeEnum>(m_data.Type);
            responsibleType.Text = m_data.Text;
            responsibleType.Type = typeEnum;
            
            m_catalogValueRepository.Save(responsibleType);

            return responsibleType.Id;
        }
    }
}