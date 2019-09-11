using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works.ResourceGroup;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class NamedResourceGroupManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly IMapper m_mapper;

        public NamedResourceGroupManager(ResourceRepository resourceRepository, IMapper mapper)
        {
            m_resourceRepository = resourceRepository;
            m_mapper = mapper;
        }

        public List<NamedResourceGroupContract> GetResourceGroupList(long projectId, ResourceTypeEnumContract? filterResourceType)
        {
            var resourceType = m_mapper.Map<ResourceTypeEnum?>(filterResourceType);
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetNamedResourceGroupList(projectId, resourceType));
            var result = m_mapper.Map<List<NamedResourceGroupContract>>(dbResult);
            return result;
        }

        public long CreateResourceGroup(long projectId, NamedResourceGroupContract newResourceGroup)
        {
            var createResourceGroupWork = new CreateNamedResourceGroupWork(m_resourceRepository, projectId, newResourceGroup, m_mapper);
            var resultId = createResourceGroupWork.Execute();
            return resultId;
        }

        public void UpdateResourceGroup(long resourceGroupId, NamedResourceGroupContract resourceGroup)
        {
            var updateResourceGroupWork = new UpdateNamedResourceGroupWork(m_resourceRepository, resourceGroupId, resourceGroup, m_mapper);
            updateResourceGroupWork.Execute();
        }

        public void DeleteResourceGroup(long resourceGroupId)
        {
            var deleteResourceGroupWork = new DeleteNamedResourceGroupWork(m_resourceRepository, resourceGroupId);
            deleteResourceGroupWork.Execute();
        }
    }
}