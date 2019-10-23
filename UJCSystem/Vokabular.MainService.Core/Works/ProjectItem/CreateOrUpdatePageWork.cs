using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class CreateOrUpdatePageWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly CreatePageContract m_pageData;
        private readonly long? m_projectId;
        private readonly long? m_resourceId;
        private readonly int m_userId;

        public CreateOrUpdatePageWork(ResourceRepository resourceRepository, CreatePageContract pageData, long? projectId, long? resourceId, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_pageData = pageData;
            m_projectId = projectId;
            m_resourceId = resourceId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            
            if ((m_projectId == null && m_resourceId == null) || (m_projectId != null && m_resourceId != null))
            {
                throw new MainServiceException(MainServiceErrorCode.ProjectIdOrResourceId, "Exactly one parameter (ProjectId or ResourceId) has to be specified");
            }

            var pageResource = new PageResource
            {
                Name = m_pageData.Name,
                Comment = m_pageData.Comment,
                Position = m_pageData.Position,
                Resource = null,
                VersionNumber = 0,
                CreateTime = now,
                CreatedByUser = user,
                Terms = null, // Terms must be also updated
            };

            if (m_resourceId != null)
            {
                var latestPageResource = m_resourceRepository.GetLatestResourceVersion<PageResource>(m_resourceId.Value);
                if (latestPageResource == null)
                {
                    throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
                }

                pageResource.Resource = latestPageResource.Resource;
                pageResource.VersionNumber = latestPageResource.VersionNumber + 1;
                pageResource.Terms = new List<Term>(latestPageResource.Terms); // Lazy fetch
            }
            else
            {
                var resource = new Resource
                {
                    ContentType = ContentTypeEnum.Page,
                    ResourceType = ResourceTypeEnum.Page,
                    Project = m_resourceRepository.Load<Project>(m_projectId),
                };

                pageResource.Resource = resource;
                pageResource.VersionNumber = 1;
            }

            pageResource.Resource.Name = m_pageData.Name;
            pageResource.Resource.LatestVersion = pageResource;
            
            m_resourceRepository.Create(pageResource);

            ResourceId = pageResource.Resource.Id;
            VersionId = pageResource.Id;
        }

        public long VersionId { get; private set; }

        public long ResourceId { get; private set; }
    }
}
