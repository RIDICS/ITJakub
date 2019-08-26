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

        public SnapshotManager(SnapshotRepository snapshotRepository, AuthorizationManager authorizationManager)
        {
            m_snapshotRepository = snapshotRepository;
            m_authorizationManager = authorizationManager;
        }

        public SnapshotContract GetLatestPublishedSnapshot(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var latestSnapshot = m_snapshotRepository.InvokeUnitOfWork(x => x.GetLatestPublishedSnapshot(projectId));
            var snapshotContract = Mapper.Map<SnapshotContract>(latestSnapshot);
            return snapshotContract;
        }
    }
}
