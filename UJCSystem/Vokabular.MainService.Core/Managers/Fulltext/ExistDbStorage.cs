﻿using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.SearchService.DataContracts.Types;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.CriteriaItem;
using Vokabular.Shared.DataContracts.Search.OldCriteriaItem;
using HitSettingsContract = Vokabular.Shared.DataContracts.Search.OldCriteriaItem.HitSettingsContract;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class ExistDbStorage : IFulltextStorage
    {
        private readonly CommunicationProvider m_communicationProvider;

        public ExistDbStorage(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        private OutputFormatEnumContract ConvertOutputTextFormat(TextFormatEnumContract format)
        {
            switch (format)
            {
                case TextFormatEnumContract.Raw:
                    return OutputFormatEnumContract.Xml;
                case TextFormatEnumContract.Html:
                    return OutputFormatEnumContract.Html;
                case TextFormatEnumContract.Rtf:
                    return OutputFormatEnumContract.Rtf;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public ProjectType ProjectType => ProjectType.Research;

        public string GetPageText(TextResource textResource, TextFormatEnumContract format)
        {
            var outputFormat = ConvertOutputTextFormat(format);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = textResource.Resource.Project;
                var bookVersion = textResource.BookVersion;
                var result = ssc.GetBookPageByXmlId(project.ExternalId, bookVersion.ExternalId, textResource.ExternalId, "pageToHtml.xsl", outputFormat, ResourceLevelEnumContract.Shared); // TODO dynamically resolve transformation type
                return result;
            }
        }

        public string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            var outputFormat = ConvertOutputTextFormat(format);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = textResource.Resource.Project;
                var bookVersion = textResource.BookVersion;
                var result = ssc.GetEditionPageFromSearch(searchRequest.ConditionConjunction, project.ExternalId, bookVersion.ExternalId, textResource.ExternalId, "pageToHtml.xsl", outputFormat, ResourceLevelEnumContract.Shared); // TODO dynamically resolve transformation type
                return result;
            }
        }

        public string GetHeadwordText(HeadwordResource headwordResource, TextFormatEnumContract format)
        {
            var outputFormat = ConvertOutputTextFormat(format);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = headwordResource.Resource.Project;
                var bookVersion = headwordResource.BookVersion;
                var result = ssc.GetDictionaryEntryByXmlId(project.ExternalId, bookVersion.ExternalId, headwordResource.ExternalId, "dictionaryToHtml.xsl", outputFormat, ResourceLevelEnumContract.Shared); // TODO dynamically resolve transformation type
                return result;
            }
        }

        public string GetHeadwordTextFromSearch(HeadwordResource headwordResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            var outputFormat = ConvertOutputTextFormat(format);
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var project = headwordResource.Resource.Project;
                var bookVersion = headwordResource.BookVersion;
                var result = ssc.GetDictionaryEntryFromSearch(searchRequest.ConditionConjunction, project.ExternalId, bookVersion.ExternalId, headwordResource.ExternalId, "dictionaryToHtml.xsl", outputFormat, ResourceLevelEnumContract.Shared); // TODO dynamically resolve transformation type
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

        public FulltextSearchResultData SearchProjectIdByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            criteria.Add(new ResultCriteriaContract
            {
                Start = start,
                Count = count,
                Sorting = SortEnum.Title, // TODO use sorting from method parameter
                Direction = ListSortDirection.Ascending,
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

        public long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var dbResult = ssc.GetCorpusSearchResultsCount(criteria);
                return dbResult;
            }
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithBookVersionRestriction(criteria, projects);

            criteria.Add(new ResultCriteriaContract
            {
                Sorting = SortEnum.Title, // TODO use sorting from method parameter
                Direction = ListSortDirection.Ascending,
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

        public string GetEditionNote(ProjectIdentificationResult project)
        {
            using (var ssc = m_communicationProvider.GetSearchServiceClient())
            {
                var result = ssc.GetBookEditionNote(project.ProjectExternalId, project.BookVersionExternalId, "pageToHtml.xsl", OutputFormatEnumContract.Html, ResourceLevelEnumContract.Shared); // TODO dynamically resolve transformation type
                return result;
            }
        }
    }
}