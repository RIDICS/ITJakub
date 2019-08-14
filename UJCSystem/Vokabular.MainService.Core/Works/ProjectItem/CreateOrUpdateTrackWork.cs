using System;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.ProjectItem
{
    public class CreateOrUpdateTrackWork : UnitOfWorkBase
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly CreateTrackContract m_trackData;
        private readonly long? m_projectId;
        private readonly long? m_resourceId;
        private readonly int m_userId;

        public CreateOrUpdateTrackWork(ResourceRepository resourceRepository, CreateTrackContract trackData, long? projectId, long? resourceId, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_trackData = trackData;
            m_projectId = projectId;
            m_resourceId = resourceId;
            m_userId = userId;
        }

        protected override void ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var user = m_resourceRepository.Load<User>(m_userId);
            var resourceBeginningPage = m_trackData.BeginningPageId != null
                ? m_resourceRepository.Load<Resource>(m_trackData.BeginningPageId.Value)
                : null;
            var resourceChapter = m_trackData.ChapterId != null
                ? m_resourceRepository.Load<Resource>(m_trackData.ChapterId.Value)
                : null;

            if ((m_projectId == null && m_resourceId == null) || (m_projectId != null && m_resourceId != null))
            {
                throw new ArgumentException("Exactly one parameter (ProjectId or ResourceId) has to be specified");
            }

            var trackResource = m_resourceId != null
                ? m_resourceRepository.GetLatestResourceVersion<TrackResource>(m_resourceId.Value)
                : new TrackResource
                {
                    VersionNumber = 0,
                    Resource = new Resource
                    {
                        ContentType = ContentTypeEnum.AudioTrack,
                        ResourceType = ResourceTypeEnum.AudioTrack,
                        Project = m_resourceRepository.Load<Project>(m_projectId),
                    }
                };

            if (trackResource == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, "The entity was not found.");
            }

            trackResource.Name = m_trackData.Name;
            trackResource.Text = m_trackData.Text;
            trackResource.Comment = m_trackData.Comment;
            trackResource.Position = m_trackData.Position;
            trackResource.ResourceChapter = resourceChapter;
            trackResource.ResourceBeginningPage = resourceBeginningPage;
            trackResource.Resource.Name = m_trackData.Name;
            trackResource.Resource.LatestVersion = trackResource;
            trackResource.VersionNumber++;
            trackResource.CreateTime = now;
            trackResource.CreatedByUser = user;

            m_resourceRepository.Save(trackResource);

            ResourceId = trackResource.Resource.Id;
            VersionId = trackResource.Id;
        }

        public long VersionId { get; private set; }

        public long ResourceId { get; private set; }
    }
}