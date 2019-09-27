using System;
using System.IO;
using System.Net;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Content
{
    public class CreateNewImageResourceVersionWork : UnitOfWorkBase<NewResourceResultContract>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly IFileSystemManager m_fileSystemManager;
        private readonly CreateImageContract m_data;
        private readonly Stream m_fileStream;
        private readonly int m_userId;

        public CreateNewImageResourceVersionWork(ResourceRepository resourceRepository, IFileSystemManager fileSystemManager, CreateImageContract data, Stream fileStream, int userId) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_data = data;
            m_fileStream = fileStream;
            m_userId = userId;
        }

        protected override NewResourceResultContract ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;

            ImageResource latestImage;
            if (m_data.ImageId == null || m_data.OriginalVersionId == null)
            {
                var newResourceResult = TryCreateNewResource();
                latestImage = new ImageResource // simulate previous ImageResource version which doesn't exist yet
                {
                    Resource = newResourceResult.Item1,
                    ResourcePage = newResourceResult.Item2.Resource,
                    VersionNumber = 0,
                };
            }
            else
            {
                latestImage = m_resourceRepository.GetLatestResourceVersion<ImageResource>(m_data.ImageId.Value);
                if (latestImage.Id != m_data.OriginalVersionId)
                {
                    throw new MainServiceException(
                        MainServiceErrorCode.ResourceModified,
                        $"Conflict. Current latest versionId is {latestImage.Id}, but originalVersionId was specified {m_data.OriginalVersionId}",
                        HttpStatusCode.Conflict
                    );
                }
            }
            
            
            var user = m_resourceRepository.Load<User>(m_userId);
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
                ResourcePage = latestImage.ResourcePage,
                Size = 0, // Must be added after saving in FileStorageManager
                VersionNumber = latestImage.VersionNumber + 1,
            };
            newImageResource.Resource.LatestVersion = newImageResource;

            m_resourceRepository.Create(newImageResource);

            var fileInfo = m_fileSystemManager.SaveResource(ResourceType.Image, latestImage.Resource.Project.Id, m_fileStream);

            newImageResource.FileId = fileInfo.FileNameId;
            newImageResource.Size = fileInfo.FileSize;
            m_resourceRepository.Update(newImageResource);

            return new NewResourceResultContract
            {
                ResourceId = newImageResource.Resource.Id,
                ResourceVersionId = newImageResource.Id,
                VersionNumber = newImageResource.VersionNumber,
            };
        }

        private Tuple<Resource, PageResource> TryCreateNewResource()
        {
            if (m_data.ResourcePageId == null)
            {
                throw new ArgumentException("Missing required parameters. Some parameters of ImageId, OriginalVersionId and ResourcePageId are required.");
            }

            var pageId = m_data.ResourcePageId.Value;
            var latestPageVersion = m_resourceRepository.GetLatestResourceVersion<PageResource>(pageId);
            if (latestPageVersion == null)
            {
                throw new MainServiceException(MainServiceErrorCode.EntityNotFound, $"PageResource with ResourceId={pageId} was not found");
            }

            var latestImage = m_resourceRepository.GetLatestPageImage(pageId);
            if (latestImage != null)
            {
                throw new MainServiceException(MainServiceErrorCode.ChangeInConflict, $"Conflict. Image already exists for specified page with ID {pageId}.");
            }

            var newResource = new Resource
            {
                Project = latestPageVersion.Resource.Project,
                Name = m_data.FileName,
                ContentType = ContentTypeEnum.Page,
                NamedResourceGroup = null,
                ResourceType = ResourceTypeEnum.Image,
            };

            return new Tuple<Resource, PageResource>(newResource, latestPageVersion);
        }
    }
}
