using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class RemoveResourceWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_resourceId;

        public RemoveResourceWork(ResourceRepository resourceRepository, long resourceId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_resourceId = resourceId;
        }

        protected override void ExecuteWorkImplementation()
        {
            new RemoveResourceSubwork(m_resourceRepository).RemoveResource(m_resourceId);
        }
    }
}