using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Vokabular.Core.Data;
using Vokabular.Core.Search;
using Vokabular.Core.Storage;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Results;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class BookManager
    {
        private readonly CriteriaKey[] m_supportedSearchPageCriteria = {CriteriaKey.Fulltext, CriteriaKey.Heading, CriteriaKey.Sentence, CriteriaKey.Term, CriteriaKey.TokenDistance };
        private readonly MetadataRepository m_metadataRepository;
        private readonly MetadataSearchCriteriaProcessor m_metadataSearchCriteriaProcessor;
        private readonly BookRepository m_bookRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly CategoryRepository m_categoryRepository;
        private readonly Dictionary<ProjectType, IFulltextStorage> m_fulltextStorages;

        public BookManager(MetadataRepository metadataRepository, CategoryRepository categoryRepository,
            MetadataSearchCriteriaProcessor metadataSearchCriteriaProcessor, BookRepository bookRepository,
            IFulltextStorage[] fulltextStorages, FileSystemManager fileSystemManager)
        {
            m_metadataRepository = metadataRepository;
            m_metadataSearchCriteriaProcessor = metadataSearchCriteriaProcessor;
            m_bookRepository = bookRepository;
            m_fileSystemManager = fileSystemManager;
            m_fulltextStorages = fulltextStorages.ToDictionary(x => x.ProjectType);
            m_categoryRepository = categoryRepository;
        }

        private IFulltextStorage GetFulltextStorage(ProjectType projectType = ProjectType.Research)
        {
            return m_fulltextStorages[projectType];
        }

        public List<BookWithCategoriesContract> GetBooksByType(BookTypeEnumContract bookType)
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByBookType(bookTypeEnum));
            var resultList = Mapper.Map<List<BookWithCategoriesContract>>(dbMetadataList);
            return resultList;
        }

        private List<SearchResultContract> MapToSearchResult(IList<MetadataResource> dbResult,
            IList<PageCountResult> dbPageCounts, IList<PageResource> termHits)
        {
            var resultList = new List<SearchResultContract>(dbResult.Count);
            var termResultDictionary = termHits?
                .GroupBy(x => x.Resource.Project.Id)
                .ToDictionary(key => key.Key, val => val.OrderBy(x => x.Position).ToList());
            foreach (var dbMetadata in dbResult)
            {
                var resultItem = Mapper.Map<SearchResultContract>(dbMetadata);
                resultList.Add(resultItem);

                var pageCountItem = dbPageCounts.FirstOrDefault(x => x.ProjectId == dbMetadata.Resource.Project.Id);
                resultItem.PageCount = pageCountItem != null ? pageCountItem.PageCount : 0;

                if (termResultDictionary != null)
                {
                    if (!termResultDictionary.TryGetValue(dbMetadata.Resource.Project.Id, out var termPageHitList))
                    {
                        termPageHitList = new List<PageResource>();
                    }
                    
                    resultItem.TermPageHits = new SearchTermResultContract
                    {
                        PageHitsCount = termPageHitList.Count,
                        PageHits = Mapper.Map<List<PageContract>>(termPageHitList),
                    };
                }
            }

            return resultList;
        }

        private TermCriteriaConditionCreator CreateTermConditionCreatorOrDefault(SearchRequestContract request, FilteredCriterias processedCriterias)
        {
            TermCriteriaConditionCreator termCriteria = null;
            if (request.FetchTerms && request.ConditionConjunction.Any(x => x.Key == CriteriaKey.Term))
            {
                termCriteria = new TermCriteriaConditionCreator();
                termCriteria.AddCriteria(processedCriterias.MetadataCriterias);
            }

            return termCriteria;
        }

        public List<SearchResultContract> SearchByCriteria(SearchRequestContract request)
        {
            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;
            var resultCriteria = processedCriterias.ResultCriteria;

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Sort = request.Sort,
                SortDirection = request.SortDirection,
                Start = request.Start,
                Count = request.Count
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                var projectIdList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                var projectRestrictionCriteria = new NewResultRestrictionCriteriaContract
                {
                    Key = CriteriaKey.ResultRestriction,
                    ProjectIds = projectIdList
                };
                nonMetadataCriterias.Add(projectRestrictionCriteria);

                // TODO send request to fulltext DB and remove this mock:
                var mockResultProjectIdList = new List<long>(){1};

                var termCriteria = CreateTermConditionCreatorOrDefault(request, processedCriterias);
                var searchByCriteriaFulltextResultWork = new SearchByCriteriaFulltextResultWork(m_metadataRepository, mockResultProjectIdList, termCriteria);
                var dbResult = searchByCriteriaFulltextResultWork.Execute();

                var resultList = MapToSearchResult(dbResult, searchByCriteriaFulltextResultWork.PageCounts, searchByCriteriaFulltextResultWork.TermHits);
                return resultList;
            }
            else
            {
                // Search in relational DB

                var termCriteria = CreateTermConditionCreatorOrDefault(request, processedCriterias);
                var searchByCriteriaWork = new SearchByCriteriaWork(m_metadataRepository, m_bookRepository, queryCreator, termCriteria);
                var dbResult = searchByCriteriaWork.Execute();

                var resultList = MapToSearchResult(dbResult, searchByCriteriaWork.PageCounts, searchByCriteriaWork.TermHits);
                return resultList;
            }
            

            // If no book restriction is set, filter books in SQL and prepare for searching in eXistDB
            //if (nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().FirstOrDefault() == null)
            //{
            //    var databaseSearchResult = m_metadataRepository.SearchByCriteriaQuery(queryCreator);

            //    if (databaseSearchResult.Count == 0)
            //    {
            //        return new List<SearchResultContract>();
            //    }

            //    var resultContract = new ResultRestrictionCriteriaContract
            //    {
            //        ResultBooks = databaseSearchResult
            //    };
            //    nonMetadataCriterias.Add(resultContract);
            //}

            //var resultCriteriaContract = nonMetadataCriterias.OfType<ResultCriteriaContract>().FirstOrDefault();

            //Dictionary<long, List<PageDescriptionContract>> bookTermResults = null;
            //Dictionary<long, long> bookTermResultsCount = null;

            //if (filteredCriterias.NonMetadataCriterias.All(x => x.Key == CriteriaKey.ResultRestriction || x.Key == CriteriaKey.Result))
            //{
            //    // Search only in SQL
            //    var resultRestriction = nonMetadataCriterias.OfType<ResultRestrictionCriteriaContract>().First();
            //    var guidListRestriction = resultRestriction.ResultBooks.Select(x => x.Guid).ToList();
            //    var resultBookVersions = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidListRestriction, resultCriteria.Start,
            //        resultCriteria.Count, resultCriteria.Sorting, resultCriteria.Direction);
            //    var pageCounts = m_bookVersionRepository.GetBooksPageCountByGuid(guidListRestriction)
            //        .ToDictionary(x => x.BookId, x => x.Count);


            //    if (resultCriteriaContract != null && resultCriteriaContract.TermsSettingsContract != null)
            //    {
            //        var termQueryCreator = new TermCriteriaQueryCreator();
            //        termQueryCreator.AddCriteria(filteredCriterias.MetadataCriterias);

            //        var booksTermResults = m_bookVersionRepository.GetBooksTermResults(guidListRestriction, termQueryCreator);
            //        bookTermResults =
            //            booksTermResults.GroupBy(x => x.BookId, x => new PageDescriptionContract { PageName = x.PageName, PageXmlId = x.PageXmlId })
            //                .ToDictionary(x => x.Key, x => x.ToList());

            //        var booksTermResultsCount = m_bookVersionRepository.GetBooksTermResultsCount(guidListRestriction, termQueryCreator);
            //        bookTermResultsCount = booksTermResultsCount.ToDictionary(x => x.BookId, x => x.PagesCount);
            //    }

            //    var resultContractList = Mapper.Map<IList<SearchResultContract>>(resultBookVersions);

            //    foreach (var resultContract in resultContractList)
            //    {
            //        resultContract.PageCount = pageCounts[resultContract.BookId];

            //        if (bookTermResults != null)
            //        {
            //            List<PageDescriptionContract> pageDescriptionContracts;
            //            long termResultsCount;
            //            resultContract.TermsPageHits = bookTermResults.TryGetValue(resultContract.BookId, out pageDescriptionContracts)
            //                ? pageDescriptionContracts
            //                : new List<PageDescriptionContract>();
            //            resultContract.TermsPageHitsCount = bookTermResultsCount.TryGetValue(resultContract.BookId, out termResultsCount)
            //                ? Convert.ToInt32(termResultsCount)
            //                : 0;
            //        }
            //    }

            //    return resultContractList;
            //}

            //// Fulltext search
            //var searchResults = m_searchServiceClient.ListSearchEditionsResults(nonMetadataCriterias);

            //var guidList = searchResults.SearchResults.Select(x => x.BookXmlId).ToList();
            //var result = m_bookVersionRepository.GetBookVersionDetailsByGuid(guidList);
            //var resultPageCountDictionary = m_bookVersionRepository.GetBooksPageCountByGuid(guidList)
            //    .ToDictionary(x => x.BookId, x => x.Count);

            //var resultDictionary = result.ToDictionary(x => x.Book.Guid);


            //if (resultCriteriaContract != null && resultCriteriaContract.TermsSettingsContract != null)
            //{
            //    var termQueryCreator = new TermCriteriaQueryCreator();
            //    termQueryCreator.AddCriteria(filteredCriterias.MetadataCriterias);

            //    var booksTermResults = m_bookVersionRepository.GetBooksTermResults(guidList, termQueryCreator);
            //    bookTermResults =
            //        booksTermResults.GroupBy(x => x.BookId, x => new PageDescriptionContract { PageName = x.PageName, PageXmlId = x.PageXmlId })
            //            .ToDictionary(x => x.Key, x => x.ToList());

            //    var booksTermResultsCount = m_bookVersionRepository.GetBooksTermResultsCount(guidList, termQueryCreator);
            //    bookTermResultsCount = booksTermResultsCount.ToDictionary(x => x.BookId, x => x.PagesCount);
            //}

            //var searchResultFullContext = new List<SearchResultContract>();


            //foreach (var searchResult in searchResults.SearchResults)
            //{
            //    var localResult = Mapper.Map<SearchResultContract>(resultDictionary[searchResult.BookXmlId]);
            //    localResult.TotalHitCount = searchResult.TotalHitCount;
            //    localResult.Results = searchResult.Results;
            //    localResult.PageCount = resultPageCountDictionary[localResult.BookId];

            //    if (bookTermResults != null)
            //    {
            //        List<PageDescriptionContract> pageDescriptionContracts;
            //        long termResultsCount;
            //        localResult.TermsPageHits = bookTermResults.TryGetValue(localResult.BookId, out pageDescriptionContracts)
            //            ? pageDescriptionContracts
            //            : new List<PageDescriptionContract>();
            //        localResult.TermsPageHitsCount = bookTermResultsCount.TryGetValue(localResult.BookId, out termResultsCount)
            //            ? Convert.ToInt32(termResultsCount)
            //            : 0;
            //    }

            //    searchResultFullContext.Add(localResult);
            //}

            //return searchResultFullContext;
        }

        public List<AudioBookSearchResultContract> SearchAudioByCriteria(SearchRequestContract request)
        {
            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Sort = request.Sort,
                SortDirection = request.SortDirection,
                Start = request.Start,
                Count = request.Count
            };
            
            // Search only in relational DB

            var searchByCriteriaWork = new SearchAudioByCriteriaWork(m_metadataRepository, m_bookRepository, queryCreator);
            var dbResult = searchByCriteriaWork.Execute();

            //var resultList = MapToSearchResult(dbResult, searchByCriteriaWork.PageCounts);
            var resultList = new List<AudioBookSearchResultContract>(dbResult.Count);
            foreach (var dbMetadata in dbResult)
            {
                var resultItem = Mapper.Map<AudioBookSearchResultContract>(dbMetadata);
                resultList.Add(resultItem);

                if (searchByCriteriaWork.FullBookRecordingsByProjectId.TryGetValue(dbMetadata.Resource.Project.Id, out var audioList))
                {
                    resultItem.FullBookRecordings = Mapper.Map<List<AudioContract>>(audioList);
                }
                else
                {
                    resultItem.FullBookRecordings = new List<AudioContract>();
                }
            }
            
            return resultList;
        }

        public long SearchByCriteriaCount(SearchRequestContract request)
        {
            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            
            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Sort = request.Sort,
                SortDirection = request.SortDirection,
                Start = request.Start,
                Count = request.Count
            };

            var result = m_bookRepository.InvokeUnitOfWork(x => x.SearchByCriteriaQueryCount(queryCreator));
            return result;
        }

        public List<HeadwordContract> SearchHeadwordByCriteria(HeadwordSearchRequestContract request)
        {
            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;
            var resultCriteria = processedCriterias.ResultCriteria;

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Start = request.Start,
                Count = request.Count
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // TODO: Search in fulltext DB

                //var projectIdList = m_metadataRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                //var projectRestrictionCriteria = new NewResultRestrictionCriteriaContract
                //{
                //    Key = CriteriaKey.ResultRestriction,
                //    ProjectIds = projectIdList
                //};
                //nonMetadataCriterias.Add(projectRestrictionCriteria);

                //// TODO send request to fulltext DB and remove this mock:
                //var mockResultProjectIdList = new List<long>() { 1 };

                //var searchByCriteriaFulltextResultWork = new SearchByCriteriaFulltextResultWork(m_metadataRepository, mockResultProjectIdList);
                //var dbResult = searchByCriteriaFulltextResultWork.Execute();

                //var resultList = MapToSearchResult(dbResult, searchByCriteriaFulltextResultWork.PageCounts);
                //return resultList;
            }
            else
            {
                // Search in relational DB

                var searchByCriteriaWork = new SearchHeadwordByCriteriaWork(m_metadataRepository, m_bookRepository, queryCreator);
                var dbResult = searchByCriteriaWork.Execute();

                var resultList = Mapper.Map<List<HeadwordContract>>(dbResult);
                return resultList;
            }


            throw new NotImplementedException();
        }

        public long SearchHeadwordByCriteriaCount(HeadwordSearchRequestContract request)
        {
            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Start = request.Start,
                Count = request.Count
            };

            var result = m_bookRepository.InvokeUnitOfWork(x => x.SearchHeadwordByCriteriaQueryCount(queryCreator));
            return result;
        }

        public List<CorpusSearchResultContract> SearchCorpusByCriteria(CorpusSearchRequestContract request)
        {
            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Start = request.Start,
                Count = request.Count,
            };

            if (processedCriterias.NonMetadataCriterias.Count == 0)
            {
                throw new HttpErrorCodeException("Missing any fulltext criteria", HttpStatusCode.BadRequest);
            }

            // Search in fulltext DB

            var projectIdList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

            var projectRestrictionCriteria = new NewResultRestrictionCriteriaContract
            {
                Key = CriteriaKey.ResultRestriction,
                ProjectIds = projectIdList
            };
            nonMetadataCriterias.Add(projectRestrictionCriteria);

            // TODO send request to fulltext DB and remove this mock:

            var mockResults = new List<CorpusSearchResultContract>
            {
                new CorpusSearchResultContract
                {
                    Title = "Title",
                    SourceAbbreviation = "Ts",
                    RelicAbbreviation = "Tr",
                    Author = "AuthLabel",
                    BookId = 1,
                    OriginDate = "1990",
                    Notes = new List<string> {"not1", "not2"},
                    BibleVerseResultContext = new BibleVerseResultContext
                    {
                        BibleBook = "BB",
                        BibleChapter = "BC",
                        BibleVerse = "BV"
                    },
                    PageResultContext = new PageWithContextContract
                    {
                        Id = 2581,
                        VersionId = 3692,
                        Name = "25r",
                        Position = 25,
                        ContextStructure = new KwicStructure
                        {
                            After = "end of sentence",
                            Match = "word",
                            Before = "sentece start"
                        }
                    },
                    VerseResultContext = new VerseResultContext
                    {
                        VerseName = "Verse1",
                        VerseXmlId = "xml-v"
                    }
                }
            };

            return mockResults;
        }

        public long SearchCorpusByCriteriaCount(CorpusSearchRequestContract request)
        {
            // TODO implement this method and remove mock
            return 1;
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

            var audioResourceByTrackId = audioBookDetailWork.Recordings.Where(x => x.ParentResource != null)
                .GroupBy(key => key.ParentResource.Id)
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
            var resultList = new List<ChapterHierarchyContract>(dbResult.Count);
            var chaptersDictionary = new Dictionary<long, ChapterHierarchyContract>();

            foreach (var chapterResource in dbResult)
            {
                var resultChapter = Mapper.Map<ChapterHierarchyContract>(chapterResource);
                resultChapter.SubChapters = new List<ChapterHierarchyContract>();
                chaptersDictionary.Add(resultChapter.Id, resultChapter);

                if (chapterResource.ParentResource == null)
                {
                    resultList.Add(resultChapter);
                }
                else
                {
                    var parentChapter = chaptersDictionary[chapterResource.ParentResource.Id];
                    parentChapter.SubChapters.Add(resultChapter);
                }
            }
            
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

        public string GetPageText(long resourcePageId, TextFormatEnumContract format)
        {
            var textResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageText(resourcePageId));
            var textResource = textResourceList.First();
            var fulltextStorage = GetFulltextStorage();

            var result = fulltextStorage.GetPageText(textResource, format);
            return result;
        }

        public FileResultData GetPageImage(long resourcePageId)
        {
            var imageResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageImage(resourcePageId));
            var imageResource = imageResourceList.First();

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
            var fileStream = m_fileSystemManager.GetResource(audioResource.Resource.Project.Id, null, audioResource.FileId, ResourceType.Audio);

            return new FileResultData
            {
                FileName = audioResource.FileName,
                MimeType = audioResource.MimeType,
                Stream = fileStream,
                FileSize = fileStream.Length,
            };
        }

        public string GetHeadwordText(long headwordId, TextFormatEnumContract format)
        {
            var headwordResource = m_bookRepository.InvokeUnitOfWork(x => x.GetHeadwordResource(headwordId, false));
            var fulltextStorage = GetFulltextStorage();

            var result = fulltextStorage.GetHeadwordText(headwordResource, format);
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
                return x.GetHeadwordAutocomplete(query, bookTypeEnum, allCategoryIds, selectedProjectIds, DefaultValues.AutocompleteMaxCount);
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

            var termConditionCreator = new TermCriteriaConditionCreator();
            termConditionCreator.AddCriteria(termConditions);
            termConditionCreator.SetProjectIds(new[] {projectId});

            var dbResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetPagesWithTerms(termConditionCreator));

            if (fulltextConditions.Count > 0)
            {
                // TODO filter pages by fulltext conditions
                throw new NotImplementedException();
            }
            
            var result = Mapper.Map<List<PageContract>>(dbResult);
            return result;
        }
    }
}