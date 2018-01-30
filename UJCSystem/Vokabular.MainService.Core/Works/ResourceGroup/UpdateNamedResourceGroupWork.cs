using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.ResourceGroup
{
    public class UpdateNamedResourceGroupWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_resourceGroupId;
        private readonly NamedResourceGroupContract m_resourceGroupData;

        public UpdateNamedResourceGroupWork(ResourceRepository resourceRepository, long resourceGroupId, NamedResourceGroupContract resourceGroupData) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_resourceGroupId = resourceGroupId;
            m_resourceGroupData = resourceGroupData;
        }

        protected override void ExecuteWorkImplementation()
        {
            var textType = Mapper.Map<TextTypeEnum>(m_resourceGroupData.TextType);
            var resourceGroup = m_resourceRepository.FindById<NamedResourceGroup>(m_resourceGroupId);
            resourceGroup.Name = m_resourceGroupData.Name;
            resourceGroup.TextType = textType;

            m_resourceRepository.Update(resourceGroup);
        }
    }
}