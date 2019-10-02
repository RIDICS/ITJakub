using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.DataContracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Text
{
    public class CreateEmptyTextResourceWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_pageId;
        private readonly int m_userId;
        private readonly IFulltextStorage m_fulltextStorage;

        public CreateEmptyTextResourceWork(ResourceRepository resourceRepository, long pageId, int userId, IFulltextStorage fulltextStorage) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_pageId = pageId;
            m_userId = userId;
            m_fulltextStorage = fulltextStorage;
        }

        protected override long ExecuteWorkImplementation()
        {
            var timeNow = DateTime.UtcNow;
            var latestPageVersion = m_resourceRepository.GetLatestResourceVersion<PageResource>(m_pageId);
            if (latestPageVersion == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, $"PageResource with ResourceId={m_pageId} was not found");
            }

            var latestTextVersion = m_resourceRepository.GetLatestPageText(m_pageId);
            if (latestTextVersion != null)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.ChangeInConflict,
                    $"Conflict. Text already exists for specified page with ID {m_pageId}."
                );
            }

            var newResource = new Resource
            {
                Project = latestPageVersion.Resource.Project,
                Name = $"{latestPageVersion.Name}.md",
                ContentType = ContentTypeEnum.Page,
                NamedResourceGroup = null,
                ResourceType = ResourceTypeEnum.Text,
            };
            var newVersion = new TextResource
            {
                CreatedByUser = m_resourceRepository.Load<User>(m_userId),
                CreateTime = timeNow,
                ExternalId = null,
                ResourcePage = latestPageVersion.Resource,
                Resource = newResource,
                VersionNumber = 1,
            };
            newVersion.Resource.LatestVersion = newVersion;

            m_resourceRepository.Create(newVersion);

            var externalTextId = m_fulltextStorage.CreateNewTextVersion(newVersion, string.Empty);

            newVersion.ExternalId = externalTextId;
            m_resourceRepository.Update(newVersion);

            return newResource.Id;
        }
    }
}