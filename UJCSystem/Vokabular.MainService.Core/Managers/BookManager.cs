using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Vokabular.Core.Search;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class BookManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly MetadataSearchCriteriaProcessor m_metadataSearchCriteriaProcessor;
        private readonly BookRepository m_bookRepository;
        private readonly CategoryRepository m_categoryRepository;

        public BookManager(MetadataRepository metadataRepository, CategoryRepository categoryRepository,
            MetadataSearchCriteriaProcessor metadataSearchCriteriaProcessor, BookRepository bookRepository)
        {
            m_metadataRepository = metadataRepository;
            m_metadataSearchCriteriaProcessor = metadataSearchCriteriaProcessor;
            m_bookRepository = bookRepository;
            m_categoryRepository = categoryRepository;
        }

        public List<BookWithCategoriesContract> GetBooksByType(BookTypeEnumContract bookType)
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByBookType(bookTypeEnum));
            var resultList = Mapper.Map<List<BookWithCategoriesContract>>(dbMetadataList);
            return resultList;
        }

        private List<SearchResultContract> MapToSearchResult(IList<MetadataResource> dbResult,
            IList<PageCountResult> dbPageCounts)
        {
            var resultList = new List<SearchResultContract>(dbResult.Count);
            foreach (var dbMetadata in dbResult)
            {
                var resultItem = Mapper.Map<SearchResultContract>(dbMetadata);
                resultList.Add(resultItem);

                var pageCountItem = dbPageCounts.FirstOrDefault(x => x.ProjectId == dbMetadata.Resource.Project.Id);
                resultItem.PageCount = pageCountItem != null ? pageCountItem.PageCount : 0;
            }

            return resultList;
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

                var projectIdList = m_metadataRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                var projectRestrictionCriteria = new NewResultRestrictionCriteriaContract
                {
                    Key = CriteriaKey.ResultRestriction,
                    ProjectIds = projectIdList
                };
                nonMetadataCriterias.Add(projectRestrictionCriteria);

                // TODO send request to fulltext DB and remove this mock:
                var mockResultProjectIdList = new List<long>(){1};

                var searchByCriteriaFulltextResultWork = new SearchByCriteriaFulltextResultWork(m_metadataRepository, mockResultProjectIdList);
                var dbResult = searchByCriteriaFulltextResultWork.Execute();

                var resultList = MapToSearchResult(dbResult, searchByCriteriaFulltextResultWork.PageCounts);
                return resultList;
            }
            else
            {
                // Search in relational DB

                var searchByCriteriaWork = new SearchByCriteriaWork(m_metadataRepository, queryCreator);
                var dbResult = searchByCriteriaWork.Execute();

                var resultList = MapToSearchResult(dbResult, searchByCriteriaWork.PageCounts);
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

            var result = m_metadataRepository.InvokeUnitOfWork(x => x.SearchByCriteriaQueryCount(queryCreator));
            return result;
        }

        public List<HeadwordContract> SearchHeadwordByCriteria(HeadwordSearchRequestContract request)
        {
            UpdateHeadwordCategoryCriteria(request); // HACK for resolving slow query execution in some cases

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

                var searchByCriteriaWork = new SearchHeadwordByCriteriaWork(m_metadataRepository, queryCreator);
                var dbResult = searchByCriteriaWork.Execute();

                var resultList = Mapper.Map<List<HeadwordContract>>(dbResult);
                return resultList;
            }


            throw new NotImplementedException();
        }

        public long SearchHeadwordByCriteriaCount(HeadwordSearchRequestContract request)
        {
            UpdateHeadwordCategoryCriteria(request); // HACK for resolving slow query execution in some cases

            // TODO add authorization
            //m_authorizationManager.AuthorizeCriteria(searchCriteriaContracts);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Start = request.Start,
                Count = request.Count
            };

            var result = m_metadataRepository.InvokeUnitOfWork(x => x.SearchHeadwordByCriteriaQueryCount(queryCreator));
            return result;
        }

        private void UpdateHeadwordCategoryCriteria(HeadwordSearchRequestContract request)
        {
            var categoryCriteria = request.ConditionConjunction.OfType<SelectedCategoryCriteriaContract>()
                .FirstOrDefault();
            UpdateHeadwordCategoryCriteria(categoryCriteria);
        }

        private void UpdateHeadwordCategoryCriteria(SelectedCategoryCriteriaContract categoryCriteria)
        {
            if (categoryCriteria != null)
            {
                // Add at least one projectId if any categoryId is specified
                if (categoryCriteria.SelectedCategoryIds != null && categoryCriteria.SelectedCategoryIds.Count > 0 &&
                    (categoryCriteria.SelectedBookIds == null || categoryCriteria.SelectedBookIds.Count == 0))
                {
                    var projectId = m_categoryRepository.InvokeUnitOfWork(x => x.GetAnyProjectIdByCategory(categoryCriteria.SelectedCategoryIds));
                    categoryCriteria.SelectedBookIds = new List<long> { projectId };
                }
            }
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

        public List<ChapterContract> GetBookChapterList(long projectId)
        {
            var listResult = m_bookRepository.InvokeUnitOfWork(x => x.GetChapterList(projectId));
            var result = Mapper.Map<List<ChapterContract>>(listResult);
            return result;
        }

        public List<TermContract> GetPageTermList(long resourcePageId)
        {
            var listResult = m_bookRepository.InvokeUnitOfWork(x => x.GetPageTermList(resourcePageId));
            var result = Mapper.Map<List<TermContract>>(listResult);
            return result;
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

        public string GetPageText(long resourcePageId)
        {
            var textResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageText(resourcePageId));

            // TODO get text from Fulltext Service
            throw new NotImplementedException();
        }

        public Stream GetPageImage(long resourcePageId)
        {
            var imageResourceList = m_bookRepository.InvokeUnitOfWork(x => x.GetPageImage(resourcePageId));

            // TODO get image form File Storage
            throw new NotImplementedException();
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

            UpdateHeadwordCategoryCriteria(request.Category);

            var projectIds = request.Category.SelectedBookIds ?? new List<long>();
            var categoryIds = request.Category.SelectedCategoryIds ?? new List<int>();
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(request.Category.BookType);

            var result = m_bookRepository.InvokeUnitOfWork(x =>
            {
                if (categoryIds.Count > 0)
                {
                    categoryIds = m_categoryRepository.GetAllSubcategoryIds(categoryIds);
                }

                return x.GetHeadwordRowNumber(request.Query, projectIds, categoryIds, bookTypeEnum);
            });

            return result;
        }
    }
}