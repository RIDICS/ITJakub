using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Snapshot
{
    public class CreateSnapshotWork : UnitOfWorkBase<long>
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_projectId;
        private readonly int m_userId;
        private readonly IList<long> m_resourceVersionIds;
        private readonly string m_comment;
        private readonly IList<BookTypeEnum> m_bookTypes;
        private readonly BookTypeEnum m_defaultBookType;
        private readonly IFulltextStorage m_fulltextStorage;

        public CreateSnapshotWork(ProjectRepository projectRepository, ResourceRepository resourceRepository, long projectId, int userId, IList<long> resourceVersionIds,
            string comment, IList<BookTypeEnum> bookTypes, BookTypeEnum defaultBookType, IFulltextStorage fulltextStorage) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_resourceRepository = resourceRepository;
            m_projectId = projectId;
            m_userId = userId;
            m_resourceVersionIds = resourceVersionIds;
            m_comment = comment;
            m_bookTypes = bookTypes;
            m_defaultBookType = defaultBookType;
            m_fulltextStorage = fulltextStorage;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.Load<Project>(m_projectId);
            var latestSnapshot = m_projectRepository.GetLatestSnapshot(m_projectId);
            var bookTypes = m_bookTypes.Select(bookTypeEnum => m_projectRepository.GetBookTypeByEnum(bookTypeEnum)).ToList();
            var defaultBookType = m_projectRepository.GetBookTypeByEnum(m_defaultBookType);
            var resourceVersions = m_resourceVersionIds.Select(x => m_projectRepository.Load<ResourceVersion>(x)).ToList();
            var versionNumber = latestSnapshot?.VersionNumber ?? 0;

            var editionNote = m_resourceRepository.GetLatestEditionNote(m_projectId);
            resourceVersions.Add(editionNote);

            var newDbSnapshot = new DataEntities.Database.Entities.Snapshot
            {
                Project = project,
                BookTypes = bookTypes,
                DefaultBookType = defaultBookType,
                Comment = m_comment,
                CreateTime = now,
                PublishTime = now,
                CreatedByUser = user,
                VersionNumber = versionNumber + 1,
                ResourceVersions = resourceVersions
            };

            project.LatestPublishedSnapshot = newDbSnapshot;
            m_projectRepository.Update(project);

            var snapshotId = (long)m_projectRepository.Create(newDbSnapshot);

            var textResourceVersions = m_resourceRepository.GetResourceVersions<TextResource>(m_resourceVersionIds);
            var metadataResourceVersion = m_resourceRepository.GetLatestMetadata(m_projectId);
            m_fulltextStorage.CreateSnapshot(newDbSnapshot, textResourceVersions, metadataResourceVersion);

            return snapshotId;
        }
    }
}