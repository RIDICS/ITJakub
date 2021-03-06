﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
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
        private readonly AuthenticationManager m_authenticationManager;
        private readonly UserDetailManager m_userDetailManager;
        private readonly IMapper m_mapper;
        private readonly ResourceRepository m_resourceRepository;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;

        public SnapshotManager(ProjectRepository projectRepository, SnapshotRepository snapshotRepository,
            AuthenticationManager authenticationManager, UserDetailManager userDetailManager, IMapper mapper,
            ResourceRepository resourceRepository, FulltextStorageProvider fulltextStorageProvider)
        {
            m_projectRepository = projectRepository;
            m_snapshotRepository = snapshotRepository;
            m_authenticationManager = authenticationManager;
            m_userDetailManager = userDetailManager;
            m_mapper = mapper;
            m_resourceRepository = resourceRepository;
            m_fulltextStorageProvider = fulltextStorageProvider;
        }

        public SnapshotContract GetLatestPublishedSnapshot(long projectId)
        {
            var latestSnapshot = m_snapshotRepository.InvokeUnitOfWork(x => x.GetLatestPublishedSnapshot(projectId));
            var snapshotContract = m_mapper.Map<SnapshotContract>(latestSnapshot);
            return snapshotContract;
        }

        public SnapshotDetailContract GetSnapshotDetail(long snapshotId)
        {
            var snapshot = m_snapshotRepository.InvokeUnitOfWork(x => x.GetSnapshotWithResources(snapshotId));
            var snapshotContract = m_mapper.Map<SnapshotDetailContract>(snapshot);
            return snapshotContract;
        }

        public long CreateSnapshot(CreateSnapshotContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var bookTypes = m_mapper.Map<IList<BookTypeEnum>>(data.BookTypes);
            var defaultBookTypes = m_mapper.Map<BookTypeEnum>(data.DefaultBookType);

            var projectInfo = m_projectRepository.InvokeUnitOfWork(x => x.GetProject(data.ProjectId));
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectInfo.ProjectType);

            var snapshotId = new CreateSnapshotWork(m_projectRepository, m_resourceRepository, data.ProjectId, userId,
                data.ResourceVersionIds, data.Comment, bookTypes, defaultBookTypes, fulltextStorage).Execute();
            return snapshotId;
        }

        public PagedResultList<SnapshotAggregatedInfoContract> GetPublishedSnapshotWithAggregatedInfo(long projectId, int? start, int? count, string filterByComment)
        {
            var startValue = PagingHelper.GetStart(start);
            var countValue = PagingHelper.GetCount(count);

            var publishedSnapshots = m_snapshotRepository.InvokeUnitOfWork(x => x.GetPublishedSnapshots(projectId, startValue, countValue, filterByComment));
            var snapshotInfo = m_snapshotRepository.InvokeUnitOfWork(x => x.GetSnapshotsResourcesCount(publishedSnapshots.List.Select(s => s.Id).ToArray()));

            var snapshotContracts = m_mapper.Map<List<SnapshotAggregatedInfoContract>>(publishedSnapshots.List);

            foreach (var snapshotContract in snapshotContracts)
            {
                var dbSnapshot = publishedSnapshots.List.FirstOrDefault(x => x.Id == snapshotContract.Id);
                if (dbSnapshot != null)
                {
                    snapshotContract.Author = m_userDetailManager.GetUserFullName(dbSnapshot.CreatedByUser);
                    snapshotContract.ResourcesInfo = new List<SnapshotResourcesInfoContract>();

                    foreach (var aggregatedInfo in snapshotInfo.Where(x => x.Id == snapshotContract.Id))
                    {
                        snapshotContract.ResourcesInfo.Add(new SnapshotResourcesInfoContract
                        {
                            ResourceType = m_mapper.Map<ResourceTypeEnumContract>(aggregatedInfo.Type),
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