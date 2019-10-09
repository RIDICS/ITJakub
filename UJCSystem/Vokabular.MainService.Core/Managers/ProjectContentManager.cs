using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Works.Content;
using Vokabular.MainService.Core.Works.Text;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectContentManager
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly IFileSystemManager m_fileSystemManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly UserDetailManager m_userDetailManager;
        private readonly IMapper m_mapper;

        public ProjectContentManager(ResourceRepository resourceRepository, IFileSystemManager fileSystemManager,
            AuthenticationManager authenticationManager, FulltextStorageProvider fulltextStorageProvider,
            UserDetailManager userDetailManager, IMapper mapper)
        {
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_authenticationManager = authenticationManager;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_userDetailManager = userDetailManager;
            m_mapper = mapper;
        }

        public IList<ResourceWithLatestVersionContract> GetResourceList(long projectId, ResourceTypeEnumContract? resourceTypeContract)
        {
            ResourceTypeEnum? resourceType = null;
            if (resourceTypeContract.HasValue)
            {
                resourceType = m_mapper.Map<ResourceTypeEnum>(resourceTypeContract);
            }
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectLatestResources(projectId, resourceType));

            var resultList = new List<ResourceWithLatestVersionContract>();
            var userCache = new Dictionary<int, string>();
            foreach (var resource in dbResult)
            {
                var latestResourceVersion = resource.LatestVersion;
                var resourceContract = m_mapper.Map<ResourceWithLatestVersionContract>(resource);
                var userId = latestResourceVersion.CreatedByUser.Id;
                if (!userCache.TryGetValue(userId, out var userName))
                {
                     userCache.Add(userId, m_userDetailManager.GetUserFullName(latestResourceVersion.CreatedByUser));
                     userCache.TryGetValue(userId, out userName);
                }

                resourceContract.LatestVersion.Author = userName;
                resourceContract.LatestVersion.RelatedResource = GetRelatedResourceContract(latestResourceVersion);
                resultList.Add(resourceContract);
            }
            
            return resultList;
        }

        private RelatedResourceContract GetRelatedResourceContract(ResourceVersion resourceVersion)
        {
            Resource relatedResource = null;
            switch (resourceVersion)
            {
                case TextResource textResource:
                    relatedResource = m_resourceRepository
                        .InvokeUnitOfWork(x => x.GetLatestResourceVersion<PageResource>(textResource.ResourcePage.Id)).Resource;
                    break;
                case ImageResource imageResource:
                    relatedResource = m_resourceRepository
                        .InvokeUnitOfWork(x => x.GetLatestResourceVersion<PageResource>(imageResource.ResourcePage.Id)).Resource;
                    break;
                case AudioResource audioResource:
                    relatedResource = m_resourceRepository
                        .InvokeUnitOfWork(x => x.GetLatestResourceVersion<TrackResource>(audioResource.ResourceTrack.Id)).Resource;
                    break;
            }

            if (relatedResource != null)
            {
                var result = m_mapper.Map<RelatedResourceContract>(relatedResource);

                switch (relatedResource.LatestVersion)
                {
                    case PageResource pageResource:
                        result.Sequence = pageResource.Position;
                        break;
                    case TrackResource trackResource:
                        result.Sequence = trackResource.Position;
                        break;
                }

                return result;
            }

            return null;
        }

        public List<TextWithPageContract> GetTextResourceList(long projectId, long? resourceGroupId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectLatestTexts(projectId, resourceGroupId, true));
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource) x.ResourcePage.LatestVersion).Position);
            var result = m_mapper.Map<List<TextWithPageContract>>(sortedDbResult);
            return result;
        }

        public List<ImageWithPageContract> GetImageResourceList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectLatestImages(projectId, null, true));
            var sortedDbResult = dbResult.OrderBy(x => ((PageResource) x.ResourcePage.LatestVersion).Position);
            var result = m_mapper.Map<List<ImageWithPageContract>>(sortedDbResult);
            return result;
        }

        public FullTextContract GetTextResource(long textId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetTextResource(textId));
            return GetTextResource(dbResult, formatValue);
        }

        public FullTextContract GetTextResourceByPageId(long pageId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestPageText(pageId));
            return GetTextResource(dbResult, formatValue);
        }

        private FullTextContract GetTextResource(TextResource dbResult, TextFormatEnumContract formatValue)
        {
            var result = m_mapper.Map<FullTextContract>(dbResult);

            if (result != null)
            {
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(dbResult.Resource.Project.ProjectType);

                var text = fulltextStorage.GetPageText(dbResult, formatValue);
                result.Text = text;
            }
            
            return result;
        }

        public FullTextContract GetTextResourceVersion(long textVersionId, TextFormatEnumContract formatValue)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetResourceVersion<TextResource>(textVersionId, true, true));
            return GetTextResource(dbResult, formatValue);
        }

        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetCommentsForText(textId));
            var contractList = m_mapper.Map<List<GetTextCommentContract>>(dbResult);
            var result = m_userDetailManager.AddUserDetails(contractList);
            return result;
        }

        public GetTextCommentContract GetComment(long commentId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetComment(commentId));
            var contract = m_mapper.Map<GetTextCommentContract>(dbResult);
            var result = m_userDetailManager.AddUserDetails(contract);
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

        public ImageContract GetImageResourceByPageId(long pageId)
        {
            var imageResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestPageImage(pageId));
            var result = m_mapper.Map<ImageContract>(imageResource);
            return result;
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

        public NewResourceResultContract CreateNewImageVersion(CreateImageContract data, Stream stream)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var resultVersionId = new CreateNewImageResourceVersionWork(m_resourceRepository, m_fileSystemManager,
                data, stream, userId).Execute();

            return resultVersionId;
        }

        public long CreateNewAudioVersion(long audioId, CreateAudioContract data, Stream stream)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var resultVersionId = new CreateNewAudioResourceVersionWork(m_resourceRepository, m_fileSystemManager, audioId,
                data, stream, userId).Execute();

            return resultVersionId;
        }

        public long CreateNewTextResourceVersion(CreateTextRequestContract request)
        {
            var latestText = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<TextResource>(request.Id));
            var project = latestText.Resource.Project;

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(project.ProjectType);

            var userId = m_authenticationManager.GetCurrentUserId();
            var createNewTextResourceWork = new CreateNewTextResourceWork(m_resourceRepository, request, userId, fulltextStorage);
            var resultId = createNewTextResourceWork.Execute();
            return resultId;
        }

        public long CreateTextResourceOnPage(long pageId)
        {
            var latestPage = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<PageResource>(pageId));
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(latestPage.Resource.Project.ProjectType);

            var userId = m_authenticationManager.GetCurrentUserId();

            var resourceId = new CreateEmptyTextResourceWork(m_resourceRepository, pageId, userId, fulltextStorage).Execute();
            return resourceId;
        }

        public void RemoveResource(long resourceId)
        {
            var work = new RemoveResourceWork(m_resourceRepository, resourceId);
            work.Execute();
        }
    }
}