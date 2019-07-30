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

        public NamedResourceGroupManager(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public List<NamedResourceGroupContract> GetResourceGroupList(long projectId, ResourceTypeEnumContract? filterResourceType)
        {
            var resourceType = Mapper.Map<ResourceTypeEnum?>(filterResourceType);
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetNamedResourceGroupList(projectId, resourceType));
            var result = Mapper.Map<List<NamedResourceGroupContract>>(dbResult);
            return result;
        }

        public long CreateResourceGroup(long projectId, NamedResourceGroupContract newResourceGroup)
        {
            var createResourceGroupWork = new CreateNamedResourceGroupWork(m_resourceRepository, projectId, newResourceGroup);
            var resultId = createResourceGroupWork.Execute();
            return resultId;
        }

        public void UpdateResourceGroup(long resourceGroupId, NamedResourceGroupContract resourceGroup)
        {
            var updateResourceGroupWork = new UpdateNamedResourceGroupWork(m_resourceRepository, resourceGroupId, resourceGroup);
            updateResourceGroupWork.Execute();
        }

        public void DeleteResourceGroup(long resourceGroupId)
        {
            var deleteResourceGroupWork = new DeleteNamedResourceGroupWork(m_resourceRepository, resourceGroupId);
            deleteResourceGroupWork.Execute();
        }
    }
}