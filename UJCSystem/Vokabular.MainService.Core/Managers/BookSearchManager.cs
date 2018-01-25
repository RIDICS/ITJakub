using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Vokabular.Core.Data;
using Vokabular.Core.Search;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers
{
    public class BookSearchManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly MetadataSearchCriteriaProcessor m_metadataSearchCriteriaProcessor;
        private readonly BookRepository m_bookRepository;
        private readonly CorpusSearchManager m_corpusSearchManager;
        private readonly HeadwordSearchManager m_headwordSearchManager;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly AuthorizationManager m_authorizationManager;

        public BookSearchManager(MetadataRepository metadataRepository,
            MetadataSearchCriteriaProcessor metadataSearchCriteriaProcessor, BookRepository bookRepository,
            CorpusSearchManager corpusSearchManager, HeadwordSearchManager headwordSearchManager,
            FulltextStorageProvider fulltextStorageProvider, AuthorizationManager authorizationManager)
        {
            m_metadataRepository = metadataRepository;
            m_metadataSearchCriteriaProcessor = metadataSearchCriteriaProcessor;
            m_bookRepository = bookRepository;
            m_corpusSearchManager = corpusSearchManager;
            m_headwordSearchManager = headwordSearchManager;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_authorizationManager = authorizationManager;
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

        private TermCriteriaPageConditionCreator CreateTermConditionCreatorOrDefault(SearchRequestContract request, FilteredCriterias processedCriterias)
        {
            TermCriteriaPageConditionCreator termCriteria = null;
            if (request.FetchTerms && request.ConditionConjunction.Any(x => x.Key == CriteriaKey.Term))
            {
                termCriteria = new TermCriteriaPageConditionCreator();
                termCriteria.AddCriteria(processedCriterias.MetadataCriterias);
            }

            return termCriteria;
        }

        public List<SearchResultContract> SearchByCriteria(SearchRequestContract request)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Sort = request.Sort,
                SortDirection = request.SortDirection,
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCountForProject(request.Count)
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                // 1) search in metadata
                var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
                var fulltextSearchResultData = fulltextStorage.SearchProjectIdByCriteria(queryCreator.GetStart(), queryCreator.GetCount(), request.Sort, request.SortDirection, nonMetadataCriterias, projectIdentificatorList);

                // 3) load paged result
                var termCriteria = CreateTermConditionCreatorOrDefault(request, processedCriterias);
                var searchByCriteriaFulltextResultWork = new SearchByCriteriaFulltextResultWork(m_metadataRepository, fulltextSearchResultData, termCriteria);
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
        }

        public List<AudioBookSearchResultContract> SearchAudioByCriteria(SearchRequestContract request)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Sort = request.Sort,
                SortDirection = request.SortDirection,
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCountForProject(request.Count)
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
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Sort = request.Sort,
                SortDirection = request.SortDirection,
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCountForProject(request.Count)
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                // 1) search in metadata
                var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
                var result = fulltextStorage.SearchByCriteriaCount(processedCriterias.NonMetadataCriterias, projectIdentificatorList);
                return result;
            }
            else
            {
                // Search in relational DB

                var result = m_bookRepository.InvokeUnitOfWork(x => x.SearchByCriteriaQueryCount(queryCreator));
                return result;
            }
        }

        public List<HeadwordContract> SearchHeadwordByCriteria(HeadwordSearchRequestContract request)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCount(request.Count)
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                // 1) search in metadata
                var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
                var fulltextSearchResultData = fulltextStorage.SearchHeadwordByCriteria(queryCreator.GetStart(), queryCreator.GetCount(), nonMetadataCriterias, projectIdentificatorList);

                // 3) load paged result
                switch (fulltextSearchResultData.SearchResultType)
                {
                    case FulltextSearchResultType.ProjectId:
                        return m_headwordSearchManager.GetHeadwordSearchResultByStandardIds(fulltextSearchResultData.List);
                    case FulltextSearchResultType.ProjectExternalId:
                        return m_headwordSearchManager.GetHeadwordSearchResultByExternalIds(fulltextSearchResultData.List);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // Search in relational DB

                var searchByCriteriaWork = new SearchHeadwordByCriteriaWork(m_metadataRepository, m_bookRepository, queryCreator);
                var dbResult = searchByCriteriaWork.Execute();

                var resultList = Mapper.Map<List<HeadwordContract>>(dbResult);
                return resultList;
            }
        }

        public long SearchHeadwordByCriteriaCount(HeadwordSearchRequestContract request)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters)
            {
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCount(request.Count)
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                // 1) search in metadata
                var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
                var result = fulltextStorage.SearchHeadwordByCriteriaCount(processedCriterias.NonMetadataCriterias, projectIdentificatorList);
                return result;
            }
            else
            {
                // Search in relational DB

                var result = m_bookRepository.InvokeUnitOfWork(x => x.SearchHeadwordByCriteriaQueryCount(queryCreator));
                return result;
            }
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusSnapshotsByCriteria(CorpusSearchRequestContract request)
        {
            var processedCriterias = GetProcessedCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters);
            
            // Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var start = m_corpusSearchManager.GetCorpusStart(request.Start);
            var count = m_corpusSearchManager.GetCorpusCount(request.Count);

            var result = fulltextStorage.SearchCorpusSnapshotsByCriteria(start, count, nonMetadataCriterias, projectIdentificatorList);

            return result;
        }
        
        public List<CorpusSearchResultContract> SearchCorpusSnapshotByCriteria(long snapshotId, CorpusSearchRequestContract request)
        {
            var processedCriterias = GetProcessedCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;
            
            // Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var start = m_corpusSearchManager.GetCorpusStart(request.Start);
            var count = m_corpusSearchManager.GetCorpusCount(request.Count);

            var result = fulltextStorage.SearchCorpusSnapshotByCriteria(snapshotId, start, count, request.ContextLength, nonMetadataCriterias);

            switch (result.SearchResultType)
            {
                case FulltextSearchResultType.ProjectId:
                    return m_corpusSearchManager.GetCorpusSearchResultByStandardIds(result.List);
                case FulltextSearchResultType.ProjectExternalId:
                    return m_corpusSearchManager.GetCorpusSearchResultByExternalIds(result.List);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public long SearchCorpusSnapshotsByCriteriaCount(SearchRequestContractBase request)
        {
            var processedCriterias = GetProcessedCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters);

            //Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var resultCount = fulltextStorage.SearchCorpusSnapshotsByCriteriaCount(nonMetadataCriterias, projectIdentificatorList);

            return resultCount;
        }

        private IList<ProjectIdentificationResult> GetProjectIdentificatorList(List<SearchCriteriaQuery> processedCriteriasConjunctionQuery, Dictionary<string, object> processedCriteriasMetadataParameters)
        {
            var queryCreator = new SearchCriteriaQueryCreator(processedCriteriasConjunctionQuery, processedCriteriasMetadataParameters);
            var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

            return projectIdentificatorList;
        }

        private FilteredCriterias GetProcessedCriterias(IList<SearchCriteriaContract> conditionConjunction)
        {
            m_authorizationManager.AddAuthorizationCriteria(conditionConjunction);
            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(conditionConjunction);

            if (processedCriterias.NonMetadataCriterias.Count == 0)
            {
                throw new HttpErrorCodeException("Missing any fulltext criteria", HttpStatusCode.BadRequest);
            }

            return processedCriterias;
        }

        public List<CorpusSearchResultContract> SearchCorpusByCriteria(CorpusSearchRequestContract request)
        {
            var processedCriterias = GetProcessedCriterias(request.ConditionConjunction);

            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters);
            // Search in fulltext DB
            
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var start = m_corpusSearchManager.GetCorpusStart(request.Start);
            var count = m_corpusSearchManager.GetCorpusCount(request.Count);
            var result = fulltextStorage.SearchCorpusByCriteria(start, count, request.ContextLength, nonMetadataCriterias, projectIdentificatorList);

            switch (result.SearchResultType)
            {
                case FulltextSearchResultType.ProjectId:
                    return m_corpusSearchManager.GetCorpusSearchResultByStandardIds(result.List);
                case FulltextSearchResultType.ProjectExternalId:
                    return m_corpusSearchManager.GetCorpusSearchResultByExternalIds(result.List);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public long SearchCorpusByCriteriaCount(CorpusSearchRequestContract request)
        {
            var processedCriterias = GetProcessedCriterias(request.ConditionConjunction);

            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters);
            
            // Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage();
            var resultCount = fulltextStorage.SearchCorpusByCriteriaCount(nonMetadataCriterias, projectIdentificatorList);

            return resultCount;
        }
    }
}