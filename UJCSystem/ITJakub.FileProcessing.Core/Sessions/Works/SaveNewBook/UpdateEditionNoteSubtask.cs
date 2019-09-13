using System;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateEditionNoteSubtask
    {
        private readonly ResourceRepository m_resourceRepository;

        public UpdateEditionNoteSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public long? UpdateEditionNote(long projectId, long bookVersionId, int userId, BookData bookData)
        {
            if (!bookData.ContainsEditionNote)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);
            var bookVersion = m_resourceRepository.Load<BookVersionResource>(bookVersionId);

            var dbEditionNoteResource = m_resourceRepository.GetLatestEditionNote(projectId);
            if (dbEditionNoteResource == null)
            {
                var newDbResource = new Resource
                {
                    Name = "Edition note",
                    ContentType = ContentTypeEnum.None,
                    ResourceType = ResourceTypeEnum.EditionNote,
                    Project = project,
                };
                return CreateEditionNoteResource(newDbResource, 1, user, now, bookVersion);
            }
            else
            {
                return CreateEditionNoteResource(dbEditionNoteResource.Resource, dbEditionNoteResource.VersionNumber + 1, user, now, bookVersion);
            }
        }

        private long CreateEditionNoteResource(Resource resource, int version, User user, DateTime now, BookVersionResource bookVersion)
        {
            var newDbEditionNote = new EditionNoteResource
            {
                Resource = resource,
                BookVersion = bookVersion,
                ExternalId = null,
                VersionNumber = version,
                Comment = string.Empty,
                CreateTime = now,
                CreatedByUser = user,
            };
            resource.LatestVersion = newDbEditionNote;
            var resultId = m_resourceRepository.Create(newDbEditionNote);

            return (long) resultId;
        }
    }
}