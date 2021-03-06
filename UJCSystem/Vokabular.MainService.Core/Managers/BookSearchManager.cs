﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Vokabular.DataEntities.Database.ConditionCriteria;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.QueryBuilder;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.Search;
using Vokabular.ForumSite.Core.Helpers;
using Vokabular.MainService.Core.Managers.Fulltext;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.Core.Managers.Search;
using Vokabular.MainService.Core.Managers.Search.Data;
using Vokabular.MainService.Core.Utils;
using Vokabular.MainService.Core.Works.Search;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Managers
{
    public class BookSearchManager
    {
        private readonly MetadataRepository m_metadataRepository;
        private readonly SnapshotRepository m_snapshotRepository;
        private readonly ResourceRepository m_resourceRepository;
        private readonly MetadataSearchCriteriaProcessor m_metadataSearchCriteriaProcessor;
        private readonly BookViewRepository m_bookRepository;
        private readonly CorpusSearchManager m_corpusSearchManager;
        private readonly HeadwordSearchManager m_headwordSearchManager;
        private readonly FulltextStorageProvider m_fulltextStorageProvider;
        private readonly AuthorizationManager m_authorizationManager;
        private readonly ForumSiteUrlHelper m_forumSiteUrlHelper;
        private readonly IMapper m_mapper;

        public BookSearchManager(MetadataRepository metadataRepository, SnapshotRepository snapshotRepository,
            ResourceRepository resourceRepository,
            MetadataSearchCriteriaProcessor metadataSearchCriteriaProcessor, BookViewRepository bookRepository,
            CorpusSearchManager corpusSearchManager, HeadwordSearchManager headwordSearchManager,
            FulltextStorageProvider fulltextStorageProvider, AuthorizationManager authorizationManager,
            ForumSiteUrlHelper forumSiteUrlHelper, IMapper mapper)
        {
            m_metadataRepository = metadataRepository;
            m_snapshotRepository = snapshotRepository;
            m_resourceRepository = resourceRepository;
            m_metadataSearchCriteriaProcessor = metadataSearchCriteriaProcessor;
            m_bookRepository = bookRepository;
            m_corpusSearchManager = corpusSearchManager;
            m_headwordSearchManager = headwordSearchManager;
            m_fulltextStorageProvider = fulltextStorageProvider;
            m_authorizationManager = authorizationManager;
            m_forumSiteUrlHelper = forumSiteUrlHelper;
            m_mapper = mapper;
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
                var resultItem = m_mapper.Map<SearchResultContract>(dbMetadata);
                resultList.Add(resultItem);

                var pageCountItem = dbPageCounts.FirstOrDefault(x => x.ProjectId == dbMetadata.Resource.Project.Id);
                resultItem.PageCount = pageCountItem != null ? pageCountItem.PageCount : 0;

                if (dbMetadata.Resource.Project.ForumId != null)
                {
                    resultItem.ForumUrl = m_forumSiteUrlHelper.GetTopicsUrl((int) dbMetadata.Resource.Project.ForumId);
                }

                if (termResultDictionary != null)
                {
                    if (!termResultDictionary.TryGetValue(dbMetadata.Resource.Project.Id, out var termPageHitList))
                    {
                        termPageHitList = new List<PageResource>();
                    }

                    resultItem.TermPageHits = new SearchTermResultContract
                    {
                        PageHitsCount = termPageHitList.Count,
                        PageHits = m_mapper.Map<List<PageContract>>(termPageHitList),
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

        public List<SearchResultContract> SearchByCriteria(AdvancedSearchRequestContract request, ProjectTypeContract projectType)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;
            var mainProjectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);
            var projectTypeEnums = new List<ProjectTypeEnum> {mainProjectTypeEnum};

            if (request.Parameters != null && request.Parameters.IncludeAdditionalProjectTypes)
            {
                projectTypeEnums.AddRange(request.Parameters.AdditionalProjectTypes.Select(x => m_mapper.Map<ProjectTypeEnum>(x)));
            }

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectTypeEnums)
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
                if (projectIdentificatorList.Count == 0)
                {
                    return new List<SearchResultContract>();
                }

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
                var fulltextSearchResultData = fulltextStorage.SearchProjectIdByCriteria(queryCreator.GetStart(), queryCreator.GetCount(), request.Sort, request.SortDirection, nonMetadataCriterias, projectIdentificatorList);

                // 3) load paged result
                var termCriteria = CreateTermConditionCreatorOrDefault(request, processedCriterias);
                var searchByCriteriaFulltextResultWork = new SearchByCriteriaFulltextResultWork(m_metadataRepository, m_bookRepository, fulltextSearchResultData, termCriteria, mainProjectTypeEnum);
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

        public List<AudioBookSearchResultContract> SearchAudioByCriteria(SearchRequestContract request, ProjectTypeContract projectType)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectTypeEnum)
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
                var resultItem = m_mapper.Map<AudioBookSearchResultContract>(dbMetadata);
                resultList.Add(resultItem);

                if (searchByCriteriaWork.FullBookRecordingsByProjectId.TryGetValue(dbMetadata.Resource.Project.Id, out var audioList))
                {
                    resultItem.FullBookRecordings = m_mapper.Map<List<AudioContract>>(audioList);
                }
                else
                {
                    resultItem.FullBookRecordings = new List<AudioContract>();
                }
            }

            return resultList;
        }

        public long SearchByCriteriaCount(AdvancedSearchRequestContract request, ProjectTypeContract projectType)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var projectTypeEnums = new List<ProjectTypeEnum> {m_mapper.Map<ProjectTypeEnum>(projectType)};

            if (request.Parameters != null && request.Parameters.IncludeAdditionalProjectTypes)
            {
                projectTypeEnums.AddRange(request.Parameters.AdditionalProjectTypes.Select(x => m_mapper.Map<ProjectTypeEnum>(x)));
            }

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectTypeEnums)
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
                if (projectIdentificatorList.Count == 0)
                {
                    return 0;
                }

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
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

        public List<HeadwordContract> SearchHeadwordByCriteria(HeadwordSearchRequestContract request, ProjectTypeContract projectType)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectTypeEnum)
            {
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCount(request.Count)
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                // 1) search in metadata
                var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));
                if (projectIdentificatorList.Count == 0)
                {
                    return new List<HeadwordContract>();
                }

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
                var fulltextSearchResultData = fulltextStorage.SearchHeadwordByCriteria(queryCreator.GetStart(), queryCreator.GetCount(), nonMetadataCriterias, projectIdentificatorList);

                // 3) load paged result
                switch (fulltextSearchResultData.SearchResultType)
                {
                    case FulltextSearchResultType.ProjectId:
                        return m_headwordSearchManager.GetHeadwordSearchResultByStandardIds(fulltextSearchResultData.List);
                    case FulltextSearchResultType.ProjectExternalId:
                        return m_headwordSearchManager.GetHeadwordSearchResultByExternalIds(fulltextSearchResultData.List, projectTypeEnum);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // Search in relational DB

                var searchByCriteriaWork = new SearchHeadwordByCriteriaWork(m_resourceRepository, m_bookRepository, queryCreator);
                var dbResult = searchByCriteriaWork.Execute();

                var resultList = m_mapper.Map<List<HeadwordContract>>(dbResult);
                return resultList;
            }
        }

        public long SearchHeadwordByCriteriaCount(HeadwordSearchRequestContract request, ProjectTypeContract projectType)
        {
            m_authorizationManager.AddAuthorizationCriteria(request.ConditionConjunction);

            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(request.ConditionConjunction);
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectTypeEnum)
            {
                Start = PagingHelper.GetStart(request.Start),
                Count = PagingHelper.GetCount(request.Count)
            };

            if (processedCriterias.NonMetadataCriterias.Count > 0)
            {
                // Search in fulltext DB

                // 1) search in metadata
                var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));
                if (projectIdentificatorList.Count == 0)
                {
                    return 0;
                }

                // 2) search in fulltext
                var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
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

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotListByCriteria(BookPagedCorpusSearchRequestContract request,
            ProjectTypeContract projectType)
        {
            var processedCriterias = GetAuthorizatedProcessedCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectType);
            if (projectIdentificatorList.Count == 0)
            {
                return new CorpusSearchSnapshotsResultContract
                {
                    TotalCount = 0,
                    SnapshotList = new List<CorpusSearchSnapshotContract>(),
                };
            }
            
            // Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
            var start = m_corpusSearchManager.GetCorpusStart(request.Start);
            var count = m_corpusSearchManager.GetCorpusCount(request.Count);

            var result = fulltextStorage.SearchCorpusGetSnapshotListByCriteria(start, count, request.Sort, request.SortDirection, nonMetadataCriterias, projectIdentificatorList, request.FetchNumberOfResults);

            return result;
        }
        
        public List<CorpusSearchResultContract> SearchCorpusInSnapshotByCriteria(long snapshotId, BookPagedCorpusSearchInSnapshotRequestContract request)
        {
            m_authorizationManager.AuthorizeSnapshot(snapshotId);

            var processedCriterias = GetAuthorizatedProcessedCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;
            
            // Search in fulltext DB
            var snapshotInfo = m_snapshotRepository.InvokeUnitOfWork(x => x.GetSnapshot(snapshotId));
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(snapshotInfo.Project.ProjectType);
            var start = m_corpusSearchManager.GetCorpusStart(request.Start);
            var count = m_corpusSearchManager.GetCorpusCount(request.Count);
            var contextLength = m_corpusSearchManager.GetContextLength(request.ContextLength);

            var result = fulltextStorage.SearchCorpusInSnapshotByCriteria(snapshotId, start, count, contextLength, nonMetadataCriterias);

            switch (result.SearchResultType)
            {
                case FulltextSearchResultType.ProjectId:
                    return m_corpusSearchManager.GetCorpusSearchResultByStandardIds(result.List);
                case FulltextSearchResultType.ProjectExternalId:
                    return m_corpusSearchManager.GetCorpusSearchResultByExternalIds(result.List, snapshotInfo.Project.ProjectType);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public long SearchCorpusTotalResultCount(SearchRequestContractBase request, ProjectTypeContract projectType)
        {
            var processedCriterias = GetAuthorizatedProcessedCriterias(request.ConditionConjunction);
            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectType);
            if (projectIdentificatorList.Count == 0)
            {
                return 0;
            }

            //Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
            var resultCount = fulltextStorage.SearchCorpusTotalResultCount(nonMetadataCriterias, projectIdentificatorList);

            return resultCount;
        }

        private IList<ProjectIdentificationResult> GetProjectIdentificatorList(List<SearchCriteriaQuery> processedCriteriasConjunctionQuery, Dictionary<string, object> processedCriteriasMetadataParameters, ProjectTypeContract projectType)
        {
            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);

            var queryCreator = new SearchCriteriaQueryCreator(processedCriteriasConjunctionQuery, processedCriteriasMetadataParameters, projectTypeEnum);
            var projectIdentificatorList = m_bookRepository.InvokeUnitOfWork(x => x.SearchProjectIdByCriteriaQuery(queryCreator));

            return projectIdentificatorList;
        }

        private FilteredCriterias GetAuthorizatedProcessedCriterias(IList<SearchCriteriaContract> conditionConjunction)
        {
            m_authorizationManager.AddAuthorizationCriteria(conditionConjunction);
            var processedCriterias = m_metadataSearchCriteriaProcessor.ProcessSearchCriterias(conditionConjunction);

            if (processedCriterias.NonMetadataCriterias.Count == 0)
            {
                throw new MainServiceException(MainServiceErrorCode.MissingFulltextCriteria,  "Missing any fulltext criteria");
            }

            return processedCriterias;
        }

        public List<CorpusSearchResultContract> SearchCorpusByCriteria(CorpusSearchRequestContract request, ProjectTypeContract projectType)
        {
            var processedCriterias = GetAuthorizatedProcessedCriterias(request.ConditionConjunction);

            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectType);
            if (projectIdentificatorList.Count == 0)
            {
                return new List<CorpusSearchResultContract>();
            }

            // Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
            var start = m_corpusSearchManager.GetCorpusStart(request.Start);
            var count = m_corpusSearchManager.GetCorpusCount(request.Count);
            var result = fulltextStorage.SearchCorpusByCriteria(start, count, request.ContextLength, request.Sort, request.SortDirection, nonMetadataCriterias, projectIdentificatorList);

            var projectTypeEnum = m_mapper.Map<ProjectTypeEnum>(projectType);
            switch (result.SearchResultType)
            {
                case FulltextSearchResultType.ProjectId:
                    return m_corpusSearchManager.GetCorpusSearchResultByStandardIds(result.List);
                case FulltextSearchResultType.ProjectExternalId:
                    return m_corpusSearchManager.GetCorpusSearchResultByExternalIds(result.List, projectTypeEnum);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public long SearchCorpusByCriteriaCount(CorpusSearchRequestContract request, ProjectTypeContract projectType)
        {
            var processedCriterias = GetAuthorizatedProcessedCriterias(request.ConditionConjunction);

            var nonMetadataCriterias = processedCriterias.NonMetadataCriterias;

            var projectIdentificatorList = GetProjectIdentificatorList(processedCriterias.ConjunctionQuery, processedCriterias.MetadataParameters, projectType);
            if (projectIdentificatorList.Count == 0)
            {
                return 0;
            }
            
            // Search in fulltext DB
            var fulltextStorage = m_fulltextStorageProvider.GetFulltextStorage(projectType);
            var resultCount = fulltextStorage.SearchCorpusByCriteriaCount(nonMetadataCriterias, projectIdentificatorList);

            return resultCount;
        }
    }
}