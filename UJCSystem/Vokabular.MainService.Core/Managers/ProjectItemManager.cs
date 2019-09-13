using System.Collections.Generic;
using AutoMapper;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.ProjectItem;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class ProjectItemManager
    {
        private readonly AuthenticationManager m_authenticationManager;
        private readonly ResourceRepository m_resourceRepository;
        private readonly BookRepository m_bookRepository;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly IMapper m_mapper;

        public ProjectItemManager(AuthenticationManager authenticationManager, ResourceRepository resourceRepository,
            BookRepository bookRepository, FulltextStorageProvider fulltextStorageProvider, FileSystemManager fileSystemManager,
            IMapper mapper)
        {
            m_authenticationManager = authenticationManager;
            m_resourceRepository = resourceRepository;
            m_bookRepository = bookRepository;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_fileSystemManager = fileSystemManager;
            m_mapper = mapper;
        }

        public List<PageContract> GetPageList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectPages(projectId));
            var result = m_mapper.Map<List<PageContract>>(dbResult);
            return result;
        }

        public PageContract GetPage(long pageId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<PageResource>(pageId));
            var result = m_mapper.Map<PageContract>(dbResult);
            return result;
        }

        public long CreatePage(long projectId, CreatePageContract pageData)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateOrUpdatePageWork(m_resourceRepository, pageData, projectId, null, userId);
            work.Execute();

            return work.ResourceId;
        }

        public void UpdatePage(long pageId, CreatePageContract pageData)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateOrUpdatePageWork(m_resourceRepository, pageData, null, pageId, userId);
            work.Execute();
        }

        public List<TermContract> GetPageTermList(long pageId)
        {
            var dbResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageTermList(pageId));
            var result = m_mapper.Map<List<TermContract>>(dbResult);
            return result;
        }

        public void SetPageTerms(long pageId, IList<int> termIdList)
        {
            new SetPageTermsWork(m_resourceRepository, pageId, termIdList).Execute();
        }

        public List<ChapterHierarchyContract> GetChapterList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectChapters(projectId));
            var result = ChaptersHelper.ChapterToHierarchyContracts(dbResult, m_mapper);
            return result;
        }

        public GetChapterContract GetChapterResource(long chapterId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<ChapterResource>(chapterId));
            var result = m_mapper.Map<GetChapterContract>(dbResult);
            return result;
        }

        public long CreateChapterResource(long projectId, CreateChapterContract chapterData)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateOrUpdateChapterWork(m_resourceRepository, chapterData, projectId, null, userId);
            work.Execute();

            return work.ResourceId;
        }

        public void UpdateChapterResource(long chapterId, CreateChapterContract chapterData)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new CreateOrUpdateChapterWork(m_resourceRepository, chapterData, null, chapterId, userId).Execute();
        }

        public List<TrackContract> GetTrackList(long projectId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetProjectTracks(projectId));
            var result = m_mapper.Map<List<TrackContract>>(dbResult);
            return result;
        }

        public TrackContract GetTrackResource(long trackId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestResourceVersion<TrackResource>(trackId));
            var result = m_mapper.Map<TrackContract>(dbResult);
            return result;
        }

        public IList<AudioContract> GetTrackRecordings(long trackId)
        {
            var dbResult = m_resourceRepository.InvokeUnitOfWork(x => x.GetRecordings(trackId));
            var result = m_mapper.Map<IList<AudioContract>>(dbResult);
            return result;
        }

        public FileResultData GetAudio(long audioId)
        {
            var audioResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetResourceVersion<AudioResource>(audioId, true, true));
            if (audioResource == null)
            {
                return null;
            }

            var fileStream =
                m_fileSystemManager.GetResource(audioResource.Resource.Project.Id, null, audioResource.FileId, ResourceType.Audio);

            return new FileResultData
            {
                FileName = audioResource.FileName,
                MimeType = audioResource.MimeType,
                Stream = fileStream,
                FileSize = fileStream.Length,
            };
        }

        public long CreateTrackResource(long projectId, CreateTrackContract trackData)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateOrUpdateTrackWork(m_resourceRepository, trackData, projectId, null, userId);
            work.Execute();

            return work.ResourceId;
        }

        public void UpdateTrackResource(long trackId, CreateTrackContract trackData)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            new CreateOrUpdateTrackWork(m_resourceRepository, trackData, null, trackId, userId).Execute();
        }
        
        public void UpdatePages(long projectId, List<CreateOrUpdatePageContract> data)
        {
            var userId = m_authenticationManager.GetCurrentUserId();
            var work = new CreateOrUpdatePagesWork(m_resourceRepository, data, projectId, userId);
            work.Execute();
        }

        public string GetPageText(long resourcePageId, TextFormatEnumContract format)
        {
            var textResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestPageText(resourcePageId));
            if (textResource == null)
            {
                return null;
            }

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(textResource.Resource.Project.ProjectType);
            return fulltextStorage.GetPageText(textResource, format);
        }

        public bool HasBookPageImage(long resourcePageId)
        {
            var imageResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestPageImage(resourcePageId));
            return imageResource != null;
        }


        public FileResultData GetPageImage(long resourcePageId)
        {
            var imageResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestPageImage(resourcePageId));
            if (imageResource == null)
            {
                return null;
            }

            var imageStream =
                m_fileSystemManager.GetResource(imageResource.Resource.Project.Id, null, imageResource.FileId, ResourceType.Image);
            return new FileResultData
            {
                FileName = imageResource.FileName,
                MimeType = imageResource.MimeType,
                Stream = imageStream,
                FileSize = imageStream.Length,
            };
        }
    }
}
