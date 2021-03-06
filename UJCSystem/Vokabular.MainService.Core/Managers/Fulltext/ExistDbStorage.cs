﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ITJakub.SearchService.DataContracts.Types;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Communication;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.OldCriteriaItem;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.TextConverter.Markdown.Extensions;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class ExistDbStorage : IFulltextStorage
    {
        private readonly CommunicationProvider m_communicationProvider;
        private readonly BookViewRepository m_bookRepository;
        private readonly IMapper m_mapper;

        public ExistDbStorage(CommunicationProvider communicationProvider, BookViewRepository bookRepository, IMapper mapper)
        {
            m_communicationProvider = communicationProvider;
            m_bookRepository = bookRepository;
            m_mapper = mapper;
        }

        private OutputFormatEnum ConvertOutputTextFormat(TextFormatEnumContract format)
        {
            switch (format)
            {
                case TextFormatEnumContract.Raw:
                    return OutputFormatEnum.Xml;
                case TextFormatEnumContract.Html:
                    return OutputFormatEnum.Html;
                case TextFormatEnumContract.Rtf:
                    return OutputFormatEnum.Rtf;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private SortEnum ConvertSortType(SortTypeEnumContract? sortType)
        {
            if (sortType == null)
            {
                return SortEnum.Title;
            }

            switch (sortType.Value)
            {
                case SortTypeEnumContract.Author:
                    return SortEnum.Author;
                case SortTypeEnumContract.Title:
                    return SortEnum.Title;
                case SortTypeEnumContract.Dating:
                    return SortEnum.Dating;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ListSortDirection ConvertSortDirection(SortDirectionEnumContract? sortDirection)
        {
            if (sortDirection == null)
            {
                return ListSortDirection.Ascending;
            }

            switch (sortDirection.Value)
            {
                case SortDirectionEnumContract.Asc:
                    return ListSortDirection.Ascending;
                case SortDirectionEnumContract.Desc:
                    return ListSortDirection.Descending;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private TransformationData GetTransformationOrDefault(TextFormatEnumContract format, BookTypeEnum bookType)
        {
            var outputFormat = ConvertOutputTextFormat(format);
            var dbTtransformation = m_bookRepository.InvokeUnitOfWork(x => x.GetDefaultTransformation(outputFormat, bookType));
            var transformation = m_mapper.Map<TransformationData>(dbTtransformation);
            if (transformation == null)
            {
                transformation = new TransformationData
                {
                    Name = "pageToHtml.xsl",
                    OutputFormat = OutputFormatEnumContract.Html,
                    ResourceLevel = ResourceLevelEnumContract.Shared,
                };
            }
            return transformation;
        }

        public FulltextStorageType StorageType => FulltextStorageType.ExistDb;

        public string GetPageText(TextResource textResource, TextFormatEnumContract format)
        {
            var transformation = GetTransformationOrDefault(format, BookTypeEnum.Edition);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = textResource.Resource.Project;
                var bookVersion = textResource.BookVersion;
                //var result = ssc.GetBookPageByXmlId(project.ExternalId, bookVersion.ExternalId, textResource.ExternalId, "pageToHtml.xsl", outputFormat, ResourceLevelEnumContract.Shared); // TODO dynamically resolve transformation type
                var result = ssc.GetBookPageByXmlId(project.ExternalId, bookVersion.ExternalId, textResource.ExternalId, transformation.Name, transformation.OutputFormat, transformation.ResourceLevel);
                return result;
            }
        }

        public string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            var transformation = GetTransformationOrDefault(format, BookTypeEnum.Edition);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = textResource.Resource.Project;
                var bookVersion = textResource.BookVersion;
                var result = ssc.GetEditionPageFromSearch(searchRequest.ConditionConjunction, project.ExternalId, bookVersion.ExternalId, textResource.ExternalId, transformation.Name, transformation.OutputFormat, transformation.ResourceLevel);
                return result;
            }
        }

        public string GetHeadwordText(HeadwordResource headwordResource, TextFormatEnumContract format)
        {
            var transformation = GetTransformationOrDefault(format, BookTypeEnum.Dictionary);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = headwordResource.Resource.Project;
                var bookVersion = headwordResource.BookVersion;
                var result = ssc.GetDictionaryEntryByXmlId(project.ExternalId, bookVersion.ExternalId, headwordResource.ExternalId, transformation.Name, transformation.OutputFormat, transformation.ResourceLevel);
                return result;
            }
        }

        public string GetHeadwordTextFromSearch(HeadwordResource headwordResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            var transformation = GetTransformationOrDefault(format, BookTypeEnum.Dictionary);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = headwordResource.Resource.Project;
                var bookVersion = headwordResource.BookVersion;
                var result = ssc.GetDictionaryEntryFromSearch(searchRequest.ConditionConjunction, project.ExternalId, bookVersion.ExternalId, headwordResource.ExternalId, transformation.Name, transformation.OutputFormat, transformation.ResourceLevel);
                return result;
            }
        }

        private void UpdateCriteriaWithBookVersionRestriction(List<SearchCriteriaContract> criteria,
            IList<ProjectIdentificationResult> projects)
        {
            var resultBooks = projects.Select(x => new BookVersionPairContract
            {
                Guid = x.ProjectExternalId,
                VersionId = x.BookVersionExternalId,
            }).ToList();

            var bookVersionRestrictionCriteria = new ResultRestrictionCriteriaContract
            {
                ResultBooks = resultBooks,
            };
            criteria.Add(bookVersionRestrictionCriteria);
        }

        public long SearchByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var result = ssc.GetSearchCriteriaResultsCount(criteria);
                return result;
            }
        }

        public FulltextSearchResultData SearchProjectIdByCriteria(int start, int count, SortTypeEnumContract? sort,
            SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            criteria.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = ConvertSortType(sort),
                Direction = ConvertSortDirection(sortDirection),
            });

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.ListSearchEditionsResults(criteria);
                var projectIds = dbResult.SearchResults.Select(x => x.BookXmlId)
                    .ToList();

                return new FulltextSearchResultData
                {
                    StringList = projectIds,
                    SearchResultType = FulltextSearchResultType.ProjectExternalId,
                };
            }
        }

        public PageSearchResultData SearchPageByCriteria(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, new List<ProjectIdentificationResult> {project});

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.GetSearchEditionsPageList(criteria);
                var projectIds = dbResult.PageList.Select(x => x.PageXmlId).ToList();

                return new PageSearchResultData
                {
                    StringList = projectIds,
                    SearchResultType = PageSearchResultType.TextExternalId,
                };
            }
        }

        public long SearchHitsResultCount(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, new List<ProjectIdentificationResult> { project });

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.ListSearchEditionsResults(criteria);
                var bookResult = dbResult.SearchResults.FirstOrDefault();

                return bookResult != null ? bookResult.TotalHitCount : 0;
            }
        }

        public SearchHitsResultData SearchHitsWithPageContext(int start, int count, int contextLength, List<SearchCriteriaContract> criteria,
            ProjectIdentificationResult project)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, new List<ProjectIdentificationResult> { project });

            criteria.Add(new ResultCriteriaContract
            {
                //Start = 0,
                //Count = 5, // if count == 1 then response is empty
                //Sorting = SortEnum.Title,
                //Direction = ListSortDirection.Ascending,
                HitSettingsContract = new HitSettingsContract
                {
                    Start = start,
                    Count = count,
                    ContextLength = 50,
                }
            });

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.ListSearchEditionsResults(criteria);
                var bookResult = dbResult.SearchResults.FirstOrDefault();
                var resultList = new List<PageResultContextData>();

                if (bookResult != null)
                {
                    resultList = bookResult.Results.Select(x => new PageResultContextData
                    {
                        StringId = x.PageXmlId,
                        ContextStructure = x.ContextStructure,
                    }).ToList();
                }

                return new SearchHitsResultData
                {
                    SearchResultType = PageSearchResultType.TextExternalId,
                    ResultList = resultList
                };
            }
        }

        public long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.GetCorpusSearchResultsCount(criteria);
                return dbResult;
            }
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength, SortTypeEnumContract? sort,
            SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            criteria.Add(new ResultCriteriaContract
            {
                Sorting = ConvertSortType(sort),
                Direction = ConvertSortDirection(sortDirection),
                HitSettingsContract = new HitSettingsContract
                {
                    Start = start,
                    Count = count,
                    ContextLength = contextLength,
                }
            });

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.GetCorpusSearchResults(criteria);

                var resultList = dbResult.SearchResults.Select(x => new CorpusSearchResultData
                {
                    ProjectExternalId = x.BookXmlId,
                    Notes = x.Notes,
                    PageResultContext = new CorpusSearchPageResultData
                    {
                        TextExternalId = x.PageResultContext?.PageXmlId,
                        ContextStructure = x.PageResultContext?.ContextStructure,
                    },
                    VerseResultContext = x.VerseResultContext,
                    BibleVerseResultContext = x.BibleVerseResultContext,
                }).ToList();

                return new CorpusSearchResultDataList
                {
                    SearchResultType = FulltextSearchResultType.ProjectExternalId,
                    List = resultList,
                };
            }
        }

        public long SearchHeadwordByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResultCount = ssc.ListSearchDictionariesResultsCount(criteria);
                return dbResultCount;
            }
        }

        public HeadwordSearchResultDataList SearchHeadwordByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            criteria.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
            });

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.ListSearchDictionariesResults(criteria);

                var resultList = new List<HeadwordDictionaryEntryData>();
                foreach (var headwordData in dbResult.HeadwordList)
                {
                    var headwordDictionaryEntries = headwordData.Dictionaries.Select(x => new HeadwordDictionaryEntryData
                    {
                        ProjectExternalId = x.BookXmlId,
                        HeadwordExternalId = x.EntryXmlId,
                    });
                    resultList.AddRange(headwordDictionaryEntries);
                }

                return new HeadwordSearchResultDataList
                {
                    SearchResultType = FulltextSearchResultType.ProjectExternalId,
                    List = resultList
                };
            }
        }

        public string GetEditionNote(EditionNoteResource editionNoteResource, TextFormatEnumContract format)
        {
            var projectExternalId = editionNoteResource.Resource.Project.ExternalId;
            var bookVersionExternalId = editionNoteResource.BookVersion.ExternalId;
            var transformation = GetTransformationOrDefault(format, BookTypeEnum.Edition);

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var result = ssc.GetBookEditionNote(projectExternalId, bookVersionExternalId, transformation.Name, transformation.OutputFormat, transformation.ResourceLevel);
                return result;
            }
        }

        public string CreateNewTextVersion(TextResource textResource, string text)
        {
            throw new NotSupportedException("Saving resources to eXist-db isn't supported. eXist-db storage supports only full book import.");
        }

        public string CreateNewHeadwordVersion(HeadwordResource headwordResource)
        {
            throw new NotSupportedException("Saving resources to eXist-db isn't supported. eXist-db storage supports only full book import.");
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotListByCriteria(int start, int count, SortTypeEnumContract? sort, SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects, bool fetchNumberOfResults)
        {
            throw new NotSupportedException("Paged search in corpus in eXist-db isn't supported.");
        }

        public CorpusSearchResultDataList SearchCorpusInSnapshotByCriteria(long snapshotId, int start, int count, int contextLength, List<SearchCriteriaContract> criteria)
        {
            throw new NotSupportedException("Paged search in corpus in eXist-db isn't supported.");
        }

        public long SearchCorpusTotalResultCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            throw new NotSupportedException("Paged search in corpus in eXist-db isn't supported.");
        }

        public void CreateSnapshot(Snapshot snapshot, IList<TextResource> orderedTextResources, MetadataResource metadata)
        {
            throw new NotSupportedException("Snapshot creating is not supported in eXist-db.");
        }

        public IList<MarkdownHeadingData> GetHeadingsFromPageText(TextResource textResource)
        {
            throw new NotSupportedException("Getting headings from text in eXist-db isn't supported.");
        }
    }
}