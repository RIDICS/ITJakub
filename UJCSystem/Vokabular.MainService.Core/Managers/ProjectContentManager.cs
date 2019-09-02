using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Works.Content;
using Vokabular.MainService.Core.Works.Text;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectContentManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly UserDetailManager m_userDetailManager;

        public ProjectContentManager(ResourceRepository resourceRepository, FileSystemManager fileSystemManager,
            AuthenticationManager authenticationManager, FulltextStorageProvider fulltextStorageProvider,
            CommunicationProvider communicationProvider, UserDetailManager userDetailManager)
        {
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_authenticationManager = authenticationManager;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_communicationProvider = communicationProvider;
            m_userDetailManager = userDetailManager;
        }

        public List<TextWithPageContract> GetTextResourceList(long projectId, long? resourceGroupId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectTexts(projectId, resourceGroupId, true));
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource) x.ResourcePage.LatestVersion).Position);
            var result = Mapper.Map<List<TextWithPageContract>>(sortedDbResult);
            return result;
        }

        public List<ImageWithPageContract> GetImageResourceList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectImages(projectId, null, true));
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource) x.ResourcePage.LatestVersion).Position);
            var result = Mapper.Map<List<ImageWithPageContract>>(sortedDbResult);
            return result;
        }

        public FullTextContract GetTextResource(long textId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetTextResource(textId));
            var result = Mapper.Map<FullTextContract>(dbResult);

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();

            var text = fulltextStorage.GetPageText(dbResult, formatValue);
            result.Text = text;

            return result;
        }

        public FullTextContract GetTextResourceVersion(long textVersionId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetResourceVersion<TextResource>(textVersionId, true, true));
            var result = Mapper.Map<FullTextContract>(dbResult);

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();

            var text = fulltextStorage.GetPageText(dbResult, formatValue);
            result.Text = text;

            return result;
        }

        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetCommentsForText(textId));
            var result = m_userDetailManager.AddUserDetails(Mapper.Map<List<GetTextCommentContract>>(dbResult));
            return result;
        }

        public long CreateNewComment(long textId, CreateTextCommentContract newComment)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var createNewCommentWork = new CreateNewTextCommentWork(m_resourceRepository, textId, newComment, userId);
            var resultId = createNewCommentWork.Execute();
            return resultId;
        }

        public void UpdateComment(long commentId, UpdateTextCommentContract data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new UpdateTextCommentWork(m_resourceRepository, commentId, data, userId).Execute();
        }

        public void DeleteComment(long commentId)
        {
            var deleteCommentWork = new DeleteTextCommentWork(m_resourceRepository, commentId);
            deleteCommentWork.Execute();
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

        public FileResultData GetImageResourceVersion(long imageVersionId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetResourceVersion<ImageResource>(imageVersionId, true, true));

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

            var userId = m_authenticationManager.GetCurrentUserId();
            var resultVersionId = new CreateNewImageResourceVersionWork(m_resourceRepository, imageId,
                data, fileInfo, userId).Execute();

            return resultVersionId;
        }

        public long CreateNewAudioVersion(long audioId, CreateAudioContract data, Stream stream)
        {
            var latestAudio = m_resourceRepository.GetLatestResourceVersion<AudioResource>(audioId);
            var projectId = latestAudio.Resource.Project.Id;

            var fileInfo = m_fileSystemManager.SaveResource(ResourceType.Audio, projectId, stream);

            var userId = m_authenticationManager.GetCurrentUserId();
            var resultVersionId = new CreateNewAudioResourceVersionWork(m_resourceRepository, audioId,
                data, fileInfo, userId).Execute();

            return resultVersionId;
        }

        public long CreateNewTextResourceVersion(CreateTextRequestContract request)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var createNewTextResourceWork = new CreateNewTextResourceWork(m_resourceRepository, request, userId, m_communicationProvider);
            var resultId = createNewTextResourceWork.Execute();
            return resultId;
        }
    }
}