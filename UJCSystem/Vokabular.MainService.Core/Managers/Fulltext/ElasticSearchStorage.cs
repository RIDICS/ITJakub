using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class ElasticSearchStorage : IFulltextStorage
    {
        private readonly CommunicationProvider m_communicationProvider;

        public ElasticSearchStorage(CommunicationProvider communicationProvider)
        {
            m_communicationProvider = communicationProvider;
        }

        public FulltextStorageType StorageType => FulltextStorageType.ElasticSearch;

        public string GetPageText(TextResource textResource, TextFormatEnumContract format)
        {
            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.GetTextResource(textResource.ExternalId, format);
            return result.PageText;
        }

        public string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.GetTextResourceFromSearch(textResource.ExternalId, format, searchRequest);
            return result.PageText;
        }

        public string GetHeadwordText(HeadwordResource headwordResource, TextFormatEnumContract format)
        {
            throw new System.NotImplementedException();
        }

        public string GetHeadwordTextFromSearch(HeadwordResource headwordResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            throw new System.NotImplementedException();
        }

        private void UpdateCriteriaWithSnapshotRestriction(List<SearchCriteriaContract> criteria,
            IList<ProjectIdentificationResult> projects)
        {
            var bookVersionRestrictionCriteria = new SnapshotResultRestrictionCriteriaContract
            {
                SnapshotIds = projects.Select(x => x.SnapshotId).ToList()
            };
            criteria.Add(bookVersionRestrictionCriteria);
        }

        public long SearchByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchByCriteriaCount(criteria);
            return result.Count;
        }

        public FulltextSearchResultData SearchProjectIdByCriteria(int start, int count, SortTypeEnumContract? sort,
            SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var searchRequest = new SearchRequestContract
            {
                Start = start,
                Count = count,
                Sort = sort,
                SortDirection = sortDirection,
                ConditionConjunction = criteria,
            };
            var result = fulltextServiceClient.SearchByCriteria(searchRequest);

            return new FulltextSearchResultData
            {
                LongList = result.ProjectIds,
                SearchResultType = FulltextSearchResultType.ProjectId
            };
        }

        public PageSearchResultData SearchPageByCriteria(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project)
        {
            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchPageByCriteria(project.SnapshotId, criteria);
            return new PageSearchResultData
            {
                SearchResultType = PageSearchResultType.TextExternalId,
                StringList = result.TextIdList,
            };
        }

        public long SearchHitsResultCount(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project)
        {
            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchHitsResultCount(project.SnapshotId, criteria);
            return result;
        }

        public SearchHitsResultData SearchHitsWithPageContext(int start, int count, int contextLength,
            List<SearchCriteriaContract> criteria,
            ProjectIdentificationResult project)
        {
            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchHitsWithPageContext(project.SnapshotId, start, count, contextLength, criteria);
            return new SearchHitsResultData
            {
                SearchResultType = PageSearchResultType.TextExternalId,
                ResultList = result.ResultList.Select(x => new PageResultContextData
                    {ContextStructure = x.ContextStructure, StringId = x.PageExternalId}).ToList(),
            };
        }

        public long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchCorpusByCriteriaCount(criteria);
            return result;
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength,
            List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchCorpusByCriteria(start, count, contextLength, criteria);

            var resultList = result.Select(x => new CorpusSearchResultData
            {
                ProjectId = x.ProjectId,
                Notes = x.Notes,
                PageResultContext = new CorpusSearchPageResultData
                {
                    TextExternalId = x.PageResultContext?.TextExternalId,
                    ContextStructure = x.PageResultContext?.ContextStructure,
                },
                VerseResultContext = x.VerseResultContext,
                BibleVerseResultContext = x.BibleVerseResultContext,
            }).ToList();

            return new CorpusSearchResultDataList
            {
                SearchResultType = FulltextSearchResultType.ProjectId,
                List = resultList
            };
        }

        public CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotListByCriteria(int start, int count, SortTypeEnumContract? sort,
            SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects,
            bool fetchNumberOfResults)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchCorpusGetSnapshotListByCriteria(start, count, sort, sortDirection, criteria,
                fetchNumberOfResults);
            return result;
        }

        public CorpusSearchResultDataList SearchCorpusInSnapshotByCriteria(long snapshotId, int start, int count, int contextLength,
            List<SearchCriteriaContract> criteria)
        {
            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchCorpusInSnapshotByCriteria(snapshotId, start, count, contextLength, criteria);

            var resultList = result.Select(x => new CorpusSearchResultData
            {
                ProjectId = x.ProjectId,
                Notes = x.Notes,
                PageResultContext = new CorpusSearchPageResultData
                {
                    TextExternalId = x.PageResultContext?.TextExternalId,
                    ContextStructure = x.PageResultContext?.ContextStructure,
                },
                VerseResultContext = x.VerseResultContext,
                BibleVerseResultContext = x.BibleVerseResultContext,
            }).ToList();

            return new CorpusSearchResultDataList
            {
                SearchResultType = FulltextSearchResultType.ProjectId,
                List = resultList
            };
        }

        public long SearchCorpusTotalResultCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            var result = fulltextServiceClient.SearchCorpusTotalResultCount(criteria);
            return result;
        }

        public void CreateSnapshot(Snapshot snapshot, IList<TextResource> textResources, MetadataResource metadata)
        {
            var snapshotResource = new SnapshotPageIdsResourceContract
            {
                PageIds = textResources.Select(x => x.ExternalId).ToList(),
                SnapshotId = snapshot.Id,
                ProjectId = snapshot.Project.Id,
                MetadataResource = new SnapshotMetadataResourceContract
                {
                    Title = metadata.Title,
                    SubTitle = metadata.SubTitle,
                    AuthorsLabel = metadata.AuthorsLabel,
                    RelicAbbreviation = metadata.RelicAbbreviation,
                    SourceAbbreviation = metadata.SourceAbbreviation,
                    PublishPlace = metadata.PublishPlace,
                    PublishDate = metadata.PublishDate,
                    PublisherText = metadata.PublisherText,
                    PublisherEmail = metadata.PublisherEmail,
                    Copyright = metadata.Copyright,
                    BiblText = metadata.BiblText,
                    OriginDate = metadata.OriginDate,
                    NotBefore = metadata.NotBefore,
                    NotAfter = metadata.NotAfter,
                    ManuscriptIdno = metadata.ManuscriptIdno,
                    ManuscriptSettlement = metadata.ManuscriptSettlement,
                    ManuscriptCountry = metadata.ManuscriptCountry,
                    ManuscriptRepository = metadata.ManuscriptRepository,
                    ManuscriptExtent = metadata.ManuscriptExtent,
                    ManuscriptTitle = metadata.ManuscriptTitle
                }
            };

            var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient();
            fulltextServiceClient.CreateSnapshot(snapshotResource);
        }

        public long SearchHeadwordByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            throw new System.NotImplementedException();
        }

        public HeadwordSearchResultDataList SearchHeadwordByCriteria(int start, int count, List<SearchCriteriaContract> criteria,
            IList<ProjectIdentificationResult> projects)
        {
            throw new System.NotImplementedException();
        }

        public string GetEditionNote(EditionNoteResource editionNoteResource, TextFormatEnumContract format)
        {
            throw new System.NotImplementedException();
        }

        public string CreateNewTextVersion(TextResource textResource)
        {
            // TODO implement and change usage in CreateNewTextResourceWork
            throw new System.NotImplementedException();
        }

        public string CreateNewHeadwordVersion(HeadwordResource headwordResource)
        {
            throw new System.NotImplementedException();
        }

        public string CreateNewEditionNoteVersion(EditionNoteResource editionNoteResource)
        {
            throw new System.NotImplementedException();
        }
    }
}