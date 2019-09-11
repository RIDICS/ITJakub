using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ResourceGroup
{
    public class UpdateNamedResourceGroupWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_resourceGroupId;
        private readonly NamedResourceGroupContract m_resourceGroupData;
        private readonly IMapper m_mapper;

        public UpdateNamedResourceGroupWork(ResourceRepository resourceRepository, long resourceGroupId, NamedResourceGroupContract resourceGroupData, IMapper mapper) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_resourceGroupId = resourceGroupId;
            m_resourceGroupData = resourceGroupData;
            m_mapper = mapper;
        }

        protected override void ExecuteWorkImplementation()
        {
            var textType = m_mapper.Map<TextTypeEnum>(m_resourceGroupData.TextType);
            var resourceGroup = m_resourceRepository.FindById<NamedResourceGroup>(m_resourceGroupId);
            resourceGroup.Name = m_resourceGroupData.Name;
            resourceGroup.TextType = textType;

            m_resourceRepository.Update(resourceGroup);
        }
    }
}