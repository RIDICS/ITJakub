using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class BookManager
    {
        private readonly CriteriaKey[] m_supportedSearchPageCriteria = {CriteriaKey.Fulltext, CriteriaKey.Heading, CriteriaKey.Sentence, CriteriaKey.Term, CriteriaKey.TokenDistance };
        private readonly MetadataRepository m_metadataRepository;
        private readonly BookRepository m_bookRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly CategoryRepository m_categoryRepository;

        public BookManager(MetadataRepository metadataRepository, CategoryRepository categoryRepository,
            BookRepository bookRepository, ResourceRepository resourceRepository, FileSystemManager fileSystemManager,
            FulltextStorageProvider fulltextStorageProvider)
        {
            m_metadataRepository = metadataRepository;
            m_bookRepository = bookRepository;
            m_resourceRepository = resourceRepository;
            m_fileSystemManager = fileSystemManager;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_categoryRepository = categoryRepository;
        }
        
        public List<BookWithCategoriesContract> GetBooksByType(BookTypeEnumContract bookType)
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByBookType(bookTypeEnum));
            var resultList = Mapper.Map<List<BookWithCategoriesContract>>(dbMetadataList);
            return resultList;
        }

        public BookContract GetBookInfo(long projectId)
        {
            var metadataResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetLatestMetadataResource(projectId));
            var result = Mapper.Map<BookContract>(metadataResult);
            return result;
        }

        public SearchResultDetailContract GetBookDetail(long projectId)
        {
            var metadataResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataWithDetail(projectId));
            var result = Mapper.Map<SearchResultDetailContract>(metadataResult);
            return result;
        }

        public AudioBookSearchResultContract GetAudioBookDetail(long projectId)
        {
            var audioBookDetailWork = new GetAudioBookDetailWork(m_metadataRepository, m_bookRepository, projectId);
            var dbResult = audioBookDetailWork.Execute();

            var bookInfo = Mapper.Map<AudioBookSearchResultContract>(dbResult);

            var audioResourceByTrackId = audioBookDetailWork.Recordings.Where(x => x.ResourceTrack != null)
                .GroupBy(key => key.ResourceTrack.Id)
                .ToDictionary(key => key.Key, val => val.ToList());

            var trackList = new List<TrackWithRecordingContract>(audioBookDetailWork.Tracks.Count);
            foreach (var trackResource in audioBookDetailWork.Tracks)
            {
                var track = Mapper.Map<TrackWithRecordingContract>(trackResource);
                trackList.Add(track);

                if (audioResourceByTrackId.TryGetValue(trackResource.Resource.Id, out var audioList))
                {
                    track.Recordings = Mapper.Map<List<AudioContract>>(audioList);
                }
            }

            bookInfo.Tracks = trackList;

            return bookInfo;
        }

        public List<PageContract> GetBookPageList(long projectId)
        {
            var listResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageList(projectId));
            var result = Mapper.Map<List<PageContract>>(listResult);
            return result;
        }

        public List<ChapterHierarchyContract> GetBookChapterList(long projectId)
        {
            var dbResult = m_bookRepository.InvokeUnitOfWork(x => x.GetChapterList(projectId));
            var resultList = ChaptersHelper.ChapterToHierarchyContracts(dbResult);
            
            return resultList;
        }

        public List<TermContract> GetPageTermList(long resourcePageId)
        {
            var listResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageTermList(resourcePageId));
            var result = Mapper.Map<List<TermContract>>(listResult);
            return result;
        }

        public bool HasBookAnyText(long projectId)
        {
            var bookTextCount =
                m_bookRepository.InvokeUnitOfWork(x => x.GetPublishedResourceCount<TextResource>(projectId));
            return bookTextCount > 0;
        }

        public bool HasBookAnyImage(long projectId)
        {
            var bookImageCount =
                m_bookRepository.InvokeUnitOfWork(x => x.GetPublishedResourceCount<ImageResource>(projectId));
            return bookImageCount > 0;
        }

        public bool HasBookPageText(long resourcePageId)
        {
            var textResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageText(resourcePageId));
            return textResourceList.Count > 0;
        }

        public bool HasBookPageImage(long resourcePageId)
        {
            var imageResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageImage(resourcePageId));
            return imageResourceList.Count > 0;
        }

        public string GetPageText(long resourcePageId, TextFormatEnumContract format, SearchPageRequestContract searchRequest = null)
        {
            var textResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageText(resourcePageId));
            var textResource = textResourceList.FirstOrDefault();
            if (textResource == null)
            {
                return null;
            }

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();

            var result = searchRequest == null
                ? fulltextStorage.GetPageText(textResource, format)
                : fulltextStorage.GetPageTextFromSearch(textResource, format, searchRequest);

            return result;
        }

        public FileResultData GetPageImage(long resourcePageId)
        {
            var imageResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageImage(resourcePageId));
            var imageResource = imageResourceList.FirstOrDefault();
            if (imageResource == null)
            {
                return null;
            }

            var imageStream = m_fileSystemManager.GetResource(imageResource.Resource.Project.Id, null, imageResource.FileId, ResourceType.Image);
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
            var audioResource = m_bookRepository.InvokeUnitOfWork(x => x.GetPublishedResourceVersion<AudioResource>(audioId));
            if (audioResource == null)
            {
                return null;
            }

            var fileStream = m_fileSystemManager.GetResource(audioResource.Resource.Project.Id, null, audioResource.FileId, ResourceType.Audio);

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
            var headwordResource = m_bookRepository.InvokeUnitOfWork(x => x.GetHeadwordResource(headwordId, false));
            if (headwordResource == null)
            {
                return null;
            }

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();

            var result = request == null
                ? fulltextStorage.GetHeadwordText(headwordResource, format)
                : fulltextStorage.GetHeadwordTextFromSearch(headwordResource, format, request);
            return result;
        }

        public List<string> GetHeadwordAutocomplete(string query, BookTypeEnumContract? bookType, IList<int> selectedCategoryIds, IList<long> selectedProjectIds)
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum?>(bookType);
            var result = m_bookRepository.InvokeUnitOfWork(x =>
            {
                var allCategoryIds = selectedCategoryIds.Count > 0
                    ? m_categoryRepository.GetAllSubcategoryIds(selectedCategoryIds)
                    : selectedCategoryIds;
                return x.GetHeadwordAutocomplete(query, bookTypeEnum, allCategoryIds, selectedProjectIds, DefaultValues.AutocompleteCount);
            });
            return result.ToList();
        }

        public long SearchHeadwordRowNumber(HeadwordRowNumberSearchRequestContract request)
        {
            if (request.Category.BookType == null)
                throw new ArgumentException("Null value of BookType is not supported");

            var searchHeadwordRowNumberWork = new SearchHeadwordRowNumberWork(m_bookRepository, m_categoryRepository, request);
            var result = searchHeadwordRowNumberWork.Execute();

            return result;
        }

        public List<PageContract> SearchPage(long projectId, SearchPageRequestContract request)
        {
            var termConditions = new List<SearchCriteriaContract>();
            var fulltextConditions = new List<SearchCriteriaContract>();
            foreach (var searchCriteriaContract in request.ConditionConjunction)
            {
                if (searchCriteriaContract.Key == CriteriaKey.Term)
                {
                    termConditions.Add(searchCriteriaContract);
                }
                else if (m_supportedSearchPageCriteria.Contains(searchCriteriaContract.Key))
                {
                    fulltextConditions.Add(searchCriteriaContract);
                }
                else
                {
                    throw new HttpErrorCodeException($"Not supported criteria key: {searchCriteriaContract.Key}", HttpStatusCode.BadRequest);
                }
            }

            IList<PageResource> pagesByMetadata = null;
            if (termConditions.Count > 0)
            {
                var termConditionCreator = new TermCriteriaPageConditionCreator();
                termConditionCreator.AddCriteria(termConditions);
                termConditionCreator.SetProjectIds(new[] { projectId });

                pagesByMetadata = m_metadataRepository.InvokeUnitOfWork(x => x.GetPagesWithTerms(termConditionCreator));
            }

            IList<PageResource> pagesByFulltext = null;
            if (fulltextConditions.Count > 0)
            {
                var projectIdentification = m_bookRepository.InvokeUnitOfWork(x => x.GetProjectIdentification(projectId));
                
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
                var fulltextResult = fulltextStorage.SearchPageByCriteria(fulltextConditions, projectIdentification);

                switch (fulltextResult.SearchResultType)
                {
                    case PageSearchResultType.TextId:
                        pagesByFulltext = m_bookRepository.InvokeUnitOfWork(x => x.GetPagesByTextVersionId(fulltextResult.LongList));
                        break;
                    case PageSearchResultType.TextExternalId:
                        pagesByFulltext = m_bookRepository.InvokeUnitOfWork(x => x.GetPagesByTextExternalId(fulltextResult.StringList, projectId));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            IList<PageResource> resultPages;
            if (pagesByMetadata != null && pagesByFulltext != null)
            {
                resultPages = pagesByMetadata.Intersect(pagesByFulltext)
                    .OrderBy(x => x.Position)
                    .ToList();
            }
            else if (pagesByFulltext != null)
            {
                resultPages = pagesByFulltext;
            }
            else if (pagesByMetadata != null)
            {
                resultPages = pagesByMetadata;
            }
            else
            {
                throw new ArgumentException("No supported search criteria was specified");
            }

            var result = Mapper.Map<List<PageContract>>(resultPages);
            return result;
        }

        public string GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            var editionNoteResource = m_resourceRepository.InvokeUnitOfWork(x => x.GetLatestEditionNote(projectId));
            if (editionNoteResource == null)
                return null;

            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var resultText = fulltextStorage.GetEditionNote(editionNoteResource, format);

            return resultText;
        }
    }
}