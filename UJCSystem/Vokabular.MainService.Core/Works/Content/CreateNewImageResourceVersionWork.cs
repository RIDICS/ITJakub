using System;
using System.Net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class CreateNewImageResourceVersionWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_imageId;
        private readonly CreateImageContract m_data;
        private readonly SaveResourceResult m_fileInfo;
        private readonly int m_userId;

        public CreateNewImageResourceVersionWork(ResourceRepository resourceRepository, long imageId, CreateImageContract data, SaveResourceResult fileInfo, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_imageId = imageId;
            m_data = data;
            m_fileInfo = fileInfo;
            m_userId = userId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            
            var latestImage = m_resourceRepository.GetLatestResourceVersion<ImageResource>(m_imageId);
            if (latestImage.Id != m_data.OriginalVersionId)
            {
                throw new HttpErrorCodeException($"Conflict. Current latest versionId is {latestImage.Id}, but originalVersionId was specified {m_data.OriginalVersionId}", HttpStatusCode.Conflict);
            }

            var user = m_resourceRepository.Load<User>(m_userId);
            var resourcePage = m_resourceRepository.Load<Resource>(m_data.ResourcePageId);
            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(m_data.FileName);

            var newImageResource = new ImageResource
            {
                CreateTime = now,
                CreatedByUser = user,
                Comment = m_data.Comment,
                FileId = m_fileInfo.FileNameId,
                FileName = m_data.FileName,
                MimeType = mimeType,
                Resource = latestImage.Resource,
                ResourcePage = resourcePage,
                Size = m_fileInfo.FileSize,
                VersionNumber = latestImage.VersionNumber + 1,
            };
            newImageResource.Resource.LatestVersion = newImageResource;

            return (long) m_resourceRepository.Create(newImageResource);
        }
    }
}
