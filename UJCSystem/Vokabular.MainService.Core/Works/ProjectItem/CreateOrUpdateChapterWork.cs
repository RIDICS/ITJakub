using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class CreateOrUpdateChapterWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly CreateChapterContract m_chapterData;
        private readonly long? m_projectId;
        private readonly long? m_resourceId;
        private readonly int m_userId;
        
        public CreateOrUpdateChapterWork(ResourceRepository resourceRepository, CreateChapterContract chapterData, long? projectId, long? resourceId, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_chapterData = chapterData;
            m_projectId = projectId;
            m_resourceId = resourceId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            var resourceBeginningPage = m_resourceRepository.Load<Resource>(m_chapterData.BeginningPageId);
            var resourceParentChapter = m_chapterData.ParentChapterId != null
                ? m_resourceRepository.Load<Resource>(m_chapterData.ParentChapterId.Value)
                : null;

            if ((m_projectId == null && m_resourceId == null) || (m_projectId != null && m_resourceId != null))
            {
                throw new MainServiceException(MainServiceErrorCode.ProjectIdOrResourceId, "Exactly one parameter (ProjectId or ResourceId) has to be specified");
            }

            var chapterResource = m_resourceId != null
                ? m_resourceRepository.GetLatestResourceVersion<ChapterResource>(m_resourceId.Value)
                : new ChapterResource
                {
                    VersionNumber = 0,
                    Resource = new Resource
                    {
                        ContentType = ContentTypeEnum.Chapter,
                        ResourceType = ResourceTypeEnum.Chapter,
                        Project = m_resourceRepository.Load<Project>(m_projectId),
                    }
                };

            if (chapterResource == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            chapterResource.Name = m_chapterData.Name;
            chapterResource.Comment = m_chapterData.Comment;
            chapterResource.Position = m_chapterData.Position;
            chapterResource.ParentResource = resourceParentChapter;
            chapterResource.ResourceBeginningPage = resourceBeginningPage;
            chapterResource.Resource.Name = m_chapterData.Name;
            chapterResource.Resource.LatestVersion = chapterResource;
            chapterResource.VersionNumber++;
            chapterResource.CreateTime = now;
            chapterResource.CreatedByUser = user;

            m_resourceRepository.Save(chapterResource);

            ResourceId = chapterResource.Resource.Id;
            VersionId = chapterResource.Id;
        }

        public long VersionId { get; private set; }

        public long ResourceId { get; private set; }
    }
}