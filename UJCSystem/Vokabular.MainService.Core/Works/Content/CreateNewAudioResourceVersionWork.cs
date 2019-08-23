using System;
using System.Net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class CreateNewAudioResourceVersionWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_audioId;
        private readonly CreateAudioContract m_data;
        private readonly SaveResourceResult m_fileInfo;
        private readonly int m_userId;

        public CreateNewAudioResourceVersionWork(ResourceRepository resourceRepository, long audioId, CreateAudioContract data,
            SaveResourceResult fileInfo, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_audioId = audioId;
            m_data = data;
            m_fileInfo = fileInfo;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            var latestAudio = m_resourceRepository.GetLatestResourceVersion<AudioResource>(m_audioId);
            if (latestAudio.Id != m_data.OriginalVersionId)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.ResourceModified,
                    $"Conflict. Current latest versionId is {latestAudio.Id}, but originalVersionId was specified {m_data.OriginalVersionId}",
                    HttpStatusCode.Conflict
                );
            }

            var audioType = AudioEnumHelper.FileNameToAudioType(m_data.FileName);
            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(m_data.FileName);
            var user = m_resourceRepository.Load<User>(m_userId);
            var resourceTrack = m_data.ResourceTrackId != null
                ? m_resourceRepository.Load<Resource>(m_data.ResourceTrackId.Value)
                : null;

            var newImageResource = new AudioResource
            {
                CreateTime = now,
                CreatedByUser = user,
                Comment = m_data.Comment,
                FileId = m_fileInfo.FileNameId,
                FileName = m_data.FileName,
                MimeType = mimeType,
                Resource = latestAudio.Resource,
                ResourceTrack = resourceTrack,
                AudioType = audioType,
                Duration = m_data.Duration,
                VersionNumber = latestAudio.VersionNumber + 1,
            };
            newImageResource.Resource.LatestVersion = newImageResource;

            return (long) m_resourceRepository.Create(newImageResource);
        }
    }
}