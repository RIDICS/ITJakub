using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.FileProcessing.Core.Sessions.Works
{
    public class CreateSnapshotForImportedDataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly long m_userId;
        private readonly IList<long> m_resourceVersionIds;
        private readonly string m_comment;

        public CreateSnapshotForImportedDataWork(ProjectRepository projectRepository, long projectId, long userId, IList<long> resourceVersionIds, string comment) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_userId = userId;
            m_resourceVersionIds = resourceVersionIds;
            m_comment = comment;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.GetProjectWithLatestPublishedSnapshot(m_projectId);

            var resourceVersions =
                m_resourceVersionIds.Select(x => m_projectRepository.Load<ResourceVersion>(x)).ToList();

            var newDbSnapshot = new Snapshot
            {
                Project = project,
                BookTypes = null, // TODO need specify book types
                Comment = m_comment,
                CreateTime = now,
                PublishTime = now, // TODO determine if truly publish now
                User = user,
                VersionNumber = project.LatestPublishedSnapshot.VersionNumber + 1,
                ResourceVersions = resourceVersions
            };

            m_projectRepository.Create(newDbSnapshot);
        }
    }
}