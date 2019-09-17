using System;
using System.IO;
using System.Net;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class CreateNewImageResourceVersionWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly long m_imageId;
        private readonly CreateImageContract m_data;
        private readonly Stream m_fileStream;
        private readonly int m_userId;

        public CreateNewImageResourceVersionWork(ResourceRepository resourceRepository, FileSystemManager fileSystemManager, long imageId, CreateImageContract data, Stream fileStream, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_imageId = imageId;
            m_data = data;
            m_fileStream = fileStream;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            
            var latestImage = m_resourceRepository.GetLatestResourceVersion<ImageResource>(m_imageId);
            if (latestImage.Id != m_data.OriginalVersionId)
            {
                throw new MainServiceException(
                    MainServiceErrorCode.ResourceModified,
                    $"Conflict. Current latest versionId is {latestImage.Id}, but originalVersionId was specified {m_data.OriginalVersionId}",
                    HttpStatusCode.Conflict
                );
            }

            var user = m_resourceRepository.Load<User>(m_userId);
            var resourcePage = m_resourceRepository.Load<Resource>(m_data.ResourcePageId);
            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(m_data.FileName);

            var newImageResource = new ImageResource
            {
                CreateTime = now,
                CreatedByUser = user,
                Comment = m_data.Comment,
                FileId = null, // Must be added after saving in FileStorageManager
                FileName = m_data.FileName,
                MimeType = mimeType,
                Resource = latestImage.Resource,
                ResourcePage = resourcePage,
                Size = 0, // Must be added after saving in FileStorageManager
                VersionNumber = latestImage.VersionNumber + 1,
            };
            newImageResource.Resource.LatestVersion = newImageResource;

            var resourceVersionId = (long) m_resourceRepository.Create(newImageResource);

            var fileInfo = m_fileSystemManager.SaveResource(ResourceType.Image, latestImage.Resource.Project.Id, m_fileStream);

            newImageResource.FileId = fileInfo.FileNameId;
            newImageResource.Size = fileInfo.FileSize;
            m_resourceRepository.Update(newImageResource);

            return resourceVersionId;
        }
    }
}
