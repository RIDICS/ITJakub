using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.Core.Search;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class BookManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly MetadataSearchCriteriaProcessor m_metadataSearchCriteriaProcessor;

        public BookManager(MetadataRepository metadataRepository, MetadataSearchCriteriaProcessor metadataSearchCriteriaProcessor)
        {
            m_metadataRepository = metadataRepository;
            m_metadataSearchCriteriaProcessor = metadataSearchCriteriaProcessor;
        }

        public List<BookWithCategoriesContract> GetBooksByType(BookTypeEnumContract bookType)
        {
            var bookTypeEnum = Mapper.Map<BookTypeEnum>(bookType);
            var dbMetadataList = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataByBookType(bookTypeEnum));
            var resultList = Mapper.Map<List<BookWithCategoriesContract>>(dbMetadataList);
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

            var searchByCriteriaWork = new SearchByCriteriaWork(m_metadataRepository, queryCreator);
            var dbResult = searchByCriteriaWork.Execute();
            
            var resultList = new List<SearchResultContract>(dbResult.Count);
            foreach (var dbMetadata in dbResult)
            {
                var resultItem = Mapper.Map<SearchResultContract>(dbMetadata);
                resultList.Add(resultItem);

                var pageCountItem = searchByCriteriaWork.PageCounts.FirstOrDefault(x => x.ProjectId == dbMetadata.Resource.Project.Id);
                resultItem.PageCount = pageCountItem != null ? pageCountItem.PageCount : 0;
            }

            return resultList;
            

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

        public SearchResultDetailContract GetBookDetail(long projectId)
        {
            var metadataResult = m_metadataRepository.InvokeUnitOfWork(x => x.GetMetadataWithDetail(projectId));
            var result = Mapper.Map<SearchResultDetailContract>(metadataResult);
            return result;
        }
    }
}