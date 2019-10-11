using System;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateBookVersionSubtask
    {
        private readonly ResourceRepository m_resourceRepository;

        public UpdateBookVersionSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public long UpdateBookVersion(long projectId, int userId, BookData bookData)
        {
            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbBookVersionResource = m_resourceRepository.GetLatestBookVersion(projectId);
            if (dbBookVersionResource == null)
            {
                var newDbResource = new Resource
                {
                    Name = "BookVersion",
                    ContentType = ContentTypeEnum.FullLiteraryWork,
                    ResourceType = ResourceTypeEnum.BookVersion,
                    Project = project,
                };
                return CreateBookVersionResource(newDbResource, 1, bookData.VersionXmlId, user, now);
            }
            else
            {
                return CreateBookVersionResource(dbBookVersionResource.Resource, dbBookVersionResource.VersionNumber + 1, bookData.VersionXmlId, user, now);
            }
        }

        private long CreateBookVersionResource(Resource resource, int version, string versionExternalId, User user, DateTime now)
        {
            var newDbBookVersion = new BookVersionResource
            {
                Resource = resource,
                ExternalId = versionExternalId,
                VersionNumber = version,
                Comment = string.Empty,
                CreateTime = now,
                CreatedByUser = user,
            };
            resource.LatestVersion = newDbBookVersion;
            var resultId = m_resourceRepository.Create(newDbBookVersion);

            return (long) resultId;
        }
    }
}