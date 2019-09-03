using System.Collections.Generic;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Works;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class SnapshotManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly SnapshotRepository m_snapshotRepository;
        private readonly AuthorizationManager m_authorizationManager;

        public SnapshotManager(ProjectRepository projectRepository, SnapshotRepository snapshotRepository, AuthorizationManager authorizationManager)
        {
            m_projectRepository = projectRepository;
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

        public long CreateSnapshot(long projectId, CreateSnapshotContract data)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            var bookTypes = Mapper.Map<IList<BookTypeEnum>>(data.BookTypes);
            var defaultBookTypes = Mapper.Map<BookTypeEnum>(data.DefaultBookType);
            var work = new CreateSnapshotWork(m_projectRepository, projectId, userId,  data.ResourceVersionIds, data.Comment, bookTypes, defaultBookTypes);
            work.Execute();
            return work.SnapshotId;
        }
    }
}
