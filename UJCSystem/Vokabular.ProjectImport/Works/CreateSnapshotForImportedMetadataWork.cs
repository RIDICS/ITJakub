using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.ProjectImport.Works
{
    public class CreateSnapshotForImportedMetadataWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly int m_userId;

        public CreateSnapshotForImportedMetadataWork(ProjectRepository projectRepository, long projectId, int userId) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.Load<Project>(m_projectId);
            var latestSnapshot = m_projectRepository.GetLatestSnapshot(m_projectId);

            var dbBookType = m_projectRepository.GetBookTypeByEnum(BookTypeEnum.BibliographicalItem);

            var versionNumber = latestSnapshot?.VersionNumber ?? 0;
            var newDbSnapshot = new Snapshot
            {
                Project = project,
                BookTypes = new List<BookType> { dbBookType },
                DefaultBookType = dbBookType,
                CreateTime = now,
                PublishTime = now,
                CreatedByUser = user,
                VersionNumber = versionNumber + 1
            };

            m_projectRepository.Create(newDbSnapshot);

            project.LatestPublishedSnapshot = newDbSnapshot;
            m_projectRepository.Update(project);
        }
    }
}