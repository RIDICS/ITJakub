using System.IO;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.Content;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectContentManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly UserManager m_userManager;

        public ProjectContentManager(ResourceRepository resourceRepository, FileSystemManager fileSystemManager, UserManager userManager)
        {
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_userManager = userManager;
        }

        public FileResultData GetImageResource(long imageId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<ImageResource>(imageId));

            var imageStream = m_fileSystemManager.GetResource(dbResult.Resource.Project.Id, null, dbResult.FileId, ResourceType.Image);
            return new FileResultData
            {
                FileName = dbResult.FileName,
                MimeType = dbResult.MimeType,
                Stream = imageStream,
                FileSize = imageStream.Length,
            };
        }

        public long CreateNewImageVersion(long imageId, CreateImageContract data, Stream stream)
        {
            var latestImage = m_resourceRepository.GetLatestResourceVersion<ImageResource>(imageId);
            var projectId = latestImage.Resource.Project.Id;

            var fileInfo = m_fileSystemManager.SaveResource(ResourceType.Image, projectId, stream);

            var userId = m_userManager.GetCurrentUserId();
            var resultVersionId = new CreateNewImageResourceVersionWork(m_resourceRepository, imageId,
                data, fileInfo, userId).Execute();

            return resultVersionId;
        }

        public long CreateNewAudioVersion(long audioId, CreateAudioContract data, Stream stream)
        {
            var latestAudio = m_resourceRepository.GetLatestResourceVersion<AudioResource>(audioId);
            var projectId = latestAudio.Resource.Project.Id;

            var fileInfo = m_fileSystemManager.SaveResource(ResourceType.Audio, projectId, stream);

            var userId = m_userManager.GetCurrentUserId();
            var resultVersionId = new CreateNewAudioResourceVersionWork(m_resourceRepository, audioId,
                data, fileInfo, userId).Execute();

            return resultVersionId;
        }
    }
}