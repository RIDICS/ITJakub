using System;
using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
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
                throw new ArgumentException("Exactly one parameter (ProjectId or ResourceId) has to be specified");
            }

            var pageResource = m_resourceId != null
                ? m_resourceRepository.GetLatestResourceVersion<PageResource>(m_resourceId.Value)
                : new PageResource
                {
                    VersionNumber = 0,
                    Resource = new Resource
                    {
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Page,
                        Project = m_resourceRepository.Load<Project>(m_projectId),
                    }
                };

            if (pageResource == null)
            {
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);
            }

            pageResource.Name = m_pageData.Name;
            pageResource.Comment = m_pageData.Comment;
            pageResource.Position = m_pageData.Position;
            pageResource.Resource.Name = m_pageData.Name;
            pageResource.Resource.LatestVersion = pageResource;
            pageResource.VersionNumber++;
            pageResource.CreateTime = now;
            pageResource.CreatedByUser = user;

            m_resourceRepository.Save(pageResource);

            ResourceId = pageResource.Resource.Id;
            VersionId = pageResource.Id;
        }

        public long VersionId { get; private set; }

        public long ResourceId { get; private set; }
    }
}
