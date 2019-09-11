using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Permission;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class BookManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly CommunicationProvider m_communicationProvider;
        private readonly CategoryRepository m_categoryRepository;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;
        private readonly IMapper m_mapper;
        private readonly BookTypeEnum[] m_filterBookType;

        public BookManager(MetadataRepository metadataRepository, CategoryRepository categoryRepository,
            BookRepository bookRepository, ResourceRepository resourceRepository, PermissionRepository permissionRepository,
            FileSystemManager fileSystemManager, FulltextStorageProvider fulltextStorageProvider, AuthorizationManager authorizationManager,
            AuthenticationManager authenticationManager, CommunicationProvider communicationProvider, ForumSiteUrlHelper forumSiteUrlHelper,
            IMapper mapper)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
            m_resourceRepository = resourceRepository;
            m_permissionRepository = permissionRepository;
            m_fileSystemManager = fileSystemManager;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_authorizationManager = authorizationManager;
            m_authenticationManager = authenticationManager;
            m_communicationProvider = communicationProvider;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_mapper = mapper;
            m_categoryRepository = categoryRepository;
            m_filterBookType = new[] {BookTypeEnum.CardFile};
        }

        public List<BookWithCategoriesContract> GetBooksByTypeForUser(BookTypeEnumContract bookType)
        {
            var bookTypeEnum = m_mapper.Map<BookTypeEnum>(bookType);
            var user = m_authenticationManager.GetCurrentUser(true);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByBookType(bookTypeEnum, user.Id));
            var resultList = m_mapper.Map<List<BookWithCategoriesContract>>(dbMetadataList);
            return resultList;
        }

        public List<BookContract> GetAllBooksByType(BookTypeEnumContract bookType)
        {           
            var bookTypeEnum = m_mapper.Map<BookTypeEnum>(bookType);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetAllMetadataByBookType(bookTypeEnum));
            var resultList = m_mapper.Map<List<BookContract>>(dbMetadataList);
            return resultList;
        }

        public List<BookTypeContract> GetBookTypeList()
        {
            var dbResult = m_bookRepository.InvokeUnitOfWork(x => x.GetBookTypes());
            var filteredResult = dbResult.Where(x => !m_filterBookType.Contains(x.Type));
            var result = m_mapper.Map<List<BookTypeContract>>(filteredResult);
            return result;
        }

        public List<BookContract> GetBooksForRole(int roleId, BookTypeEnumContract bookType)
        {
            new SynchronizeRoleWork(m_permissionRepository, m_communicationProvider, roleId).Execute();
            var group = m_permissionRepository.InvokeUnitOfWork(x => x.FindGroupByExternalIdOrCreate(roleId));
            var bookTypeEnum = m_mapper.Map<BookTypeEnum>(bookType);
            var dbResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataForUserGroup(bookTypeEnum, group.Id));
            var result = m_mapper.Map<List<BookContract>>(dbResult);
            return result;
        }

        public BookContract GetBookInfo(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var metadataResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetLatestMetadataResource(projectId));
            var result = m_mapper.Map<BookContract>(metadataResult);
            return result;
        }

        public BookContract GetBookInfoByExternalId(string projectExternalId, ProjectTypeContract projectType)
        {
            // Authorize after getting projectId

            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);
            var metadataResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetLatestMetadataResourceByExternalId(projectExternalId, projectTypeEnum));
            if (metadataResult == null)
                return null;

            m_authorizationManager.AuthorizeBook(metadataResult.Resource.Project.Id);

            var result = m_mapper.Map<BookContract>(metadataResult);
            return result;
        }
        
        public SearchResultDetailContract GetBookDetail(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var metadataResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataWithDetail(projectId));
            var result = m_mapper.Map<SearchResultDetailContract>(metadataResult);
            
            if (metadataResult.Resource.Project.ForumId != null)
            {
                result.ForumUrl = m_forumSiteUrlHelper.GetTopicsUrl((int)metadataResult.Resource.Project.ForumId);
            }

            return result;
        }

        public AudioBookSearchResultContract GetAudioBookDetail(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var audioBookDetailWork = new GetAudioBookDetailWork(m_metadataRepository, m_bookRepository, projectId);
            var dbResult = audioBookDetailWork.Execute();

            var bookInfo = m_mapper.Map<AudioBookSearchResultContract>(dbResult);

            var audioResourceByTrackId = audioBookDetailWork.Recordings.Where(x => x.ResourceTrack != null)
                .GroupBy(key => key.ResourceTrack.Id)
                .ToDictionary(key => key.Key, val => val.ToList());

            var trackList = new List<TrackWithRecordingContract>(audioBookDetailWork.Tracks.Count);
            foreach (var trackResource in audioBookDetailWork.Tracks)
            {
                var track = m_mapper.Map<TrackWithRecordingContract>(trackResource);
                trackList.Add(track);

                if (audioResourceByTrackId.TryGetValue(trackResource.Resource.Id, out var audioList))
                {
                    track.Recordings = m_mapper.Map<List<AudioContract>>(audioList);
                }
            }

            bookInfo.Tracks = trackList;

            return bookInfo;
        }

        public List<PageContract> GetBookPageList(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var listResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageList(projectId));
            var result = m_mapper.Map<List<PageContract>>(listResult);
            return result;
        }

        public List<ChapterHierarchyContract> GetBookChapterList(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var dbResult = m_bookRepository.InvokeUnitOfWork(x => x.GetChapterList(projectId));
            var resultList = ChaptersHelper.ChapterToHierarchyContracts(dbResult, m_mapper);
            
            return resultList;
        }

        public List<TermContract> GetPageTermList(long resourcePageId)
        {
            m_authorizationManager.AuthorizeResource(resourcePageId);

            var listResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageTermList(resourcePageId));
            var result = m_mapper.Map<List<TermContract>>(listResult);
            return result;
        }

        public bool HasBookAnyText(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var bookTextCount =
                m_bookRepository.InvokeUnitOfWork(x => x.GetPublishedResourceCount<TextResource>(projectId));
            return bookTextCount > 0;
        }

        public bool HasBookAnyImage(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var bookImageCount =
                m_bookRepository.InvokeUnitOfWork(x => x.GetPublishedResourceCount<ImageResource>(projectId));
            return bookImageCount > 0;
        }

        public bool HasBookPageText(long resourcePageId)
        {
            m_authorizationManager.AuthorizeResource(resourcePageId);

            var textResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageText(resourcePageId));
            return textResourceList.Count > 0;
        }

        public bool HasBookPageImage(long resourcePageId)
        {
            m_authorizationManager.AuthorizeResource(resourcePageId);

            var imageResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageImage(resourcePageId));
            return imageResourceList.Count > 0;
        }

        public string GetPageText(long resourcePageId, TextFormatEnumContract format, SearchPageRequestContract searchRequest = null)
        {
            m_authorizationManager.AuthorizeResource(resourcePageId);

            var textResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageText(resourcePageId));
            var textResource = textResourceList.FirstOrDefault();
            if (textResource == null)
            {
                return null;
            }

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(textResource.Resource.Project.ProjectType);

            var result = searchRequest == null
                ? fulltextStorage.GetPageText(textResource, format)
                : fulltextStorage.GetPageTextFromSearch(textResource, format, searchRequest);

            return result;
        }

        public FileResultData GetPageImage(long resourcePageId)
        {
            m_authorizationManager.AuthorizeResource(resourcePageId);

            var imageResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageImage(resourcePageId));
            var imageResource = imageResourceList.FirstOrDefault();
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

        public FileResultData GetAudio(long audioId)
        {
            m_authorizationManager.AuthorizeResource(audioId);

            var audioResource = m_bookRepository.InvokeUnitOfWork(x => x.GetPublishedResourceVersion<AudioResource>(audioId));
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

        public string GetHeadwordText(long headwordId, TextFormatEnumContract format, SearchPageRequestContract request = null)
        {
            m_authorizationManager.AuthorizeResource(headwordId);

            var headwordResource = m_bookRepository.InvokeUnitOfWork(x => x.GetHeadwordResource(headwordId, false));
            if (headwordResource == null)
            {
                return null;
            }

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(headwordResource.Resource.Project.ProjectType);

            var result = request == null
                ? fulltextStorage.GetHeadwordText(headwordResource, format)
                : fulltextStorage.GetHeadwordTextFromSearch(headwordResource, format, request);
            return result;
        }

        public List<string> GetHeadwordAutocomplete(string query, ProjectTypeContract projectType, BookTypeEnumContract? bookType,
            IList<int> selectedCategoryIds,
            IList<long> selectedProjectIds)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            var bookTypeEnum = m_mapper.Map<BookTypeEnum?>(bookType);
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);
            var result = m_bookRepository.InvokeUnitOfWork(x =>
            {
                var allCategoryIds = selectedCategoryIds.Count > 0
                    ? m_categoryRepository.GetAllSubcategoryIds(selectedCategoryIds)
                    : selectedCategoryIds;
                return x.GetHeadwordAutocomplete(query, projectTypeEnum, bookTypeEnum, allCategoryIds, selectedProjectIds, DefaultValues.AutocompleteCount,
                    userId);
            });
            return result.ToList();
        }

        public long SearchHeadwordRowNumber(HeadwordRowNumberSearchRequestContract request, ProjectTypeContract projectType)
        {
            var userId = m_authorizationManager.GetCurrentUserId();
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);

            if (request.Category.BookType == null)
                throw new MainServiceException(MainServiceErrorCode.NullBookTypeNotSupported,"Null value of BookType is not supported");

            var searchHeadwordRowNumberWork = new SearchHeadwordRowNumberWork(m_bookRepository, m_categoryRepository, request, userId, projectTypeEnum, m_mapper);
            var result = searchHeadwordRowNumberWork.Execute();

            return result;
        }

        public string GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            m_authorizationManager.AuthorizeBook(projectId);

            var editionNoteResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestEditionNote(projectId));
            if (editionNoteResource == null)
                return null;

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(editionNoteResource.Resource.Project.ProjectType);
            var resultText = fulltextStorage.GetEditionNote(editionNoteResource, format);

            return resultText;
        }
    }
}