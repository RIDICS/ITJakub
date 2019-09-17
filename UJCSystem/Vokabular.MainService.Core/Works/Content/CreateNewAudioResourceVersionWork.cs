using System;
using System.IO;
using System.Net;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class CreateNewAudioResourceVersionWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly long m_audioId;
        private readonly CreateAudioContract m_data;
        private readonly Stream m_fileStream;
        private readonly int m_userId;

        public CreateNewAudioResourceVersionWork(ResourceRepository resourceRepository, FileSystemManager fileSystemManager, long audioId,
            CreateAudioContract data, Stream fileStream, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_audioId = audioId;
            m_data = data;
            m_fileStream = fileStream;
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

            var newAudioResource = new AudioResource
            {
                CreateTime = now,
                CreatedByUser = user,
                Comment = m_data.Comment,
                FileId = null, // Must be added after saving in FileStorageManager
                FileName = m_data.FileName,
                MimeType = mimeType,
                Resource = latestAudio.Resource,
                ResourceTrack = resourceTrack,
                AudioType = audioType,
                Duration = m_data.Duration,
                VersionNumber = latestAudio.VersionNumber + 1,
            };
            newAudioResource.Resource.LatestVersion = newAudioResource;

            var resourceVersionId = (long) m_resourceRepository.Create(newAudioResource);

            var fileInfo = m_fileSystemManager.SaveResource(ResourceType.Audio, latestAudio.Resource.Project.Id, m_fileStream);

            newAudioResource.FileId = fileInfo.FileNameId;
            m_resourceRepository.Update(newAudioResource);

            return resourceVersionId;
        }
    }
}