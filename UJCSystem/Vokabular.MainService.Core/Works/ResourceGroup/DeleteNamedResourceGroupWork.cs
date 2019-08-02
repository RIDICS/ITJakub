using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ResourceGroup
{
    public class DeleteNamedResourceGroupWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_resourceGroupId;

        public DeleteNamedResourceGroupWork(ResourceRepository resourceRepository, long resourceGroupId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_resourceGroupId = resourceGroupId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var resourceGroup = m_resourceRepository.Load<NamedResourceGroup>(m_resourceGroupId);
            m_resourceRepository.Delete(resourceGroup);
        }
    }
}