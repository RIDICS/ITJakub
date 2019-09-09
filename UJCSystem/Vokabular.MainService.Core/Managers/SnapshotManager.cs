using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Snapshot;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class SnapshotManager
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly SnapshotRepository m_snapshotRepository;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly UserDetailManager m_userDetailManager;

        public SnapshotManager(ProjectRepository projectRepository, SnapshotRepository snapshotRepository,
            AuthorizationManager authorizationManager, UserDetailManager userDetailManager)
        {
            m_projectRepository = projectRepository;
            m_snapshotRepository = snapshotRepository;
            m_authorizationManager = authorizationManager;
            m_userDetailManager = userDetailManager;
        }

        public SnapshotContract GetLatestPublishedSnapshot(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var latestSnapshot = m_snapshotRepository.InvokeUnitOfWork(x => x.GetLatestPublishedSnapshot(projectId));
            var snapshotContract = Mapper.Map<SnapshotContract>(latestSnapshot);
            return snapshotContract;
        }

        public SnapshotDetailContract GetSnapshotDetail(long snapshotId)
        {
            var work = new GetSnapshotDetailWork(m_snapshotRepository, snapshotId);
            work.Execute();
            return work.Snapshot;
        }

        public long CreateSnapshot(long projectId, CreateSnapshotContract data)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            var bookTypes = Mapper.Map<IList<BookTypeEnum>>(data.BookTypes);
            var defaultBookTypes = Mapper.Map<BookTypeEnum>(data.DefaultBookType);
            var work = new CreateSnapshotWork(m_projectRepository, projectId, userId, data.ResourceVersionIds, data.Comment, bookTypes,
                defaultBookTypes);
            work.Execute();
            return work.SnapshotId;
        }

        public PagedResultList<SnapshotAggregatedInfoContract> GetPublishedSnapshotWithAggregatedInfo(long projectId, int? start, int? count, string filterByComment)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var publishedSnapshots = m_snapshotRepository.InvokeUnitOfWork(x => x.GetPublishedSnapshots(projectId, startValue, countValue, filterByComment));
            var snapshotInfo = m_snapshotRepository.InvokeUnitOfWork(x => x.GetSnapshotsResourcesCount(publishedSnapshots.List.Select(s => s.Id).ToArray()));

            var snapshotContracts = Mapper.Map<List<SnapshotAggregatedInfoContract>>(publishedSnapshots.List);

            foreach (var snapshotContract in snapshotContracts)
            {
                var dbSnapshot = publishedSnapshots.List.FirstOrDefault(x => x.Id == snapshotContract.Id);
                if (dbSnapshot != null)
                {
                    snapshotContract.Author = m_userDetailManager.GetUserFirstNameAndLastName(dbSnapshot.CreatedByUser);
                    snapshotContract.ResourcesInfo = new List<SnapshotResourcesInfoContract>();

                    foreach (var aggregatedInfo in snapshotInfo.Where(x => x.Id == snapshotContract.Id))
                    {
                        snapshotContract.ResourcesInfo.Add(new SnapshotResourcesInfoContract
                        {
                            ResourceType = Mapper.Map<ResourceTypeEnumContract>(aggregatedInfo.Type),
                            PublishedCount = aggregatedInfo.ResourcesCount
                        });
                    }
                }
            }

            return new PagedResultList<SnapshotAggregatedInfoContract>
            {
                List = snapshotContracts,
                TotalCount = publishedSnapshots.Count
            };
        }
    }
}