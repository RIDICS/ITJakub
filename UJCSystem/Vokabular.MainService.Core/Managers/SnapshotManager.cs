using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class SnapshotManager
    {
        private readonly SnapshotRepository m_snapshotRepository;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly IMapper m_mapper;

        public SnapshotManager(SnapshotRepository snapshotRepository, AuthorizationManager authorizationManager, IMapper mapper)
        {
            m_snapshotRepository = snapshotRepository;
            m_authorizationManager = authorizationManager;
            m_mapper = mapper;
        }

        public SnapshotContract GetLatestPublishedSnapshot(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var latestSnapshot = m_snapshotRepository.InvokeUnitOfWork(x => x.GetLatestPublishedSnapshot(projectId));
            var snapshotContract = m_mapper.Map<SnapshotContract>(latestSnapshot);
            return snapshotContract;
        }
    }
}
