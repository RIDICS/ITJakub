using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Snapshot
{
    public class CreateSnapshotWork : UnitOfWorkBase
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly long m_projectId;
        private readonly int m_userId;
        private readonly IList<long> m_resourceVersionIds;
        private readonly string m_comment;
        private readonly IList<BookTypeEnum> m_bookTypes;
        private readonly BookTypeEnum m_defaultBookType;

        public CreateSnapshotWork(ProjectRepository projectRepository, long projectId, int userId, IList<long> resourceVersionIds,
            string comment, IList<BookTypeEnum> bookTypes, BookTypeEnum defaultBookType) : base(projectRepository)
        {
            m_projectRepository = projectRepository;
            m_projectId = projectId;
            m_userId = userId;
            m_resourceVersionIds = resourceVersionIds;
            m_comment = comment;
            m_bookTypes = bookTypes;
            m_defaultBookType = defaultBookType;
        }

        public long SnapshotId { get; private set; }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_projectRepository.Load<User>(m_userId);
            var project = m_projectRepository.Load<Project>(m_projectId);
            var latestSnapshot = m_projectRepository.GetLatestSnapshot(m_projectId);
            var bookTypes = CreateBookTypes();
            var defaultBookType = CreateBookType(m_defaultBookType);

            var resourceVersions =
                m_resourceVersionIds.Select(x => m_projectRepository.Load<ResourceVersion>(x)).ToList();

            var versionNumber = latestSnapshot?.VersionNumber ?? 0;

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
                BookVersion = null, //bookVersionResource, TODO create booVersionResource
                ResourceVersions = resourceVersions
            };

            SnapshotId = (long) m_projectRepository.Create(newDbSnapshot);

            project.LatestPublishedSnapshot = newDbSnapshot;
            m_projectRepository.Update(project);
        }

        private IList<BookType> CreateBookTypes()
        {
            var resultList = new List<BookType>();
            foreach (var bookType in m_bookTypes)
            {
                var dbBookType = CreateBookType(bookType);
                resultList.Add(dbBookType);
            }

            return resultList;
        }

        private BookType CreateBookType(BookTypeEnum bookTypeEnum)
        {
            var dbBookType = m_projectRepository.GetBookTypeByEnum(bookTypeEnum);
            if (dbBookType == null)
            {
                dbBookType = new BookType
                {
                    Type = bookTypeEnum
                };
                m_projectRepository.Create(dbBookType);
            }

            return dbBookType;
        }
    }
}