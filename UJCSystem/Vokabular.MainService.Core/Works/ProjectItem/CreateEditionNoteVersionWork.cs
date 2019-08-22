using System;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class CreateEditionNoteVersionWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_projectId;
        private readonly CreateEditionNoteContract m_data;
        private readonly int m_userId;
        private readonly IFulltextStorage m_fulltextStorage;

        public CreateEditionNoteVersionWork(ResourceRepository resourceRepository, long projectId, CreateEditionNoteContract data,
            int userId, IFulltextStorage fulltextStorage) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_projectId = projectId;
            m_data = data;
            m_userId = userId;
            m_fulltextStorage = fulltextStorage;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            var latestEditionNote = m_resourceRepository.GetLatestEditionNote(m_projectId);

            if (latestEditionNote != null && latestEditionNote.Id != m_data.OriginalVersionId)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.EditionNoteConflict,
                    $"Conflict. Current latest versionId is {latestEditionNote.Id}, but originalVersionId was specified {m_data.OriginalVersionId}",
                    HttpStatusCode.Conflict
                );
            }

            if (latestEditionNote == null)
            {
                latestEditionNote = new EditionNoteResource
                {
                    Resource = new Resource
                    {
                        Project = m_resourceRepository.Load<Project>(m_projectId),
                        ContentType = ContentTypeEnum.None,
                        ResourceType = ResourceTypeEnum.EditionNote,
                        Name = "Edition note",
                    },
                    VersionNumber = 0,
                };
            }

            var newEditionNote = new EditionNoteResource
            {
                Resource = latestEditionNote.Resource,
                CreateTime = now,
                CreatedByUser = user,
                Comment = m_data.Comment,
                ExternalId = null, // Temporary value
                VersionNumber = latestEditionNote.VersionNumber + 1,
            };
            newEditionNote.Resource.LatestVersion = newEditionNote;

            var resourceVersionId = (long) m_resourceRepository.Create(newEditionNote);

            // Save text to external database
            var newExternalId = m_fulltextStorage.CreateNewEditionNoteVersion(newEditionNote);

            newEditionNote.ExternalId = newExternalId;
            m_resourceRepository.Update(newEditionNote);

            return resourceVersionId;
        }
    }
}