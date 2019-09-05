using AutoMapper;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Snapshot
{
    public class GetSnapshotDetailWork : UnitOfWorkBase
    {
        private readonly SnapshotRepository m_snapshotRepository;
        private readonly long m_snapshotId;

        public GetSnapshotDetailWork(SnapshotRepository snapshotRepository, long snapshotId) : base(snapshotRepository)
        {
            m_snapshotRepository = snapshotRepository;
            m_snapshotId = snapshotId;
        }

        public SnapshotDetailContract Snapshot { get; private set; }

        protected override void ExecuteWorkImplementation()
        {
            var snapshot = m_snapshotRepository.GetSnapshotWithResources(m_snapshotId);
            Snapshot = Mapper.Map<SnapshotDetailContract>(snapshot);
        }
    }
}