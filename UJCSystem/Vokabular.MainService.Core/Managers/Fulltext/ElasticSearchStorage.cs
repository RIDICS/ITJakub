using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
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

        public ProjectType ProjectType => ProjectType.Community;

        public string GetPageText(TextResource textResource, TextFormatEnumContract format)
        {
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
                var result = fulltextServiceClient.GetTextResource(textResource.ExternalId, format);
                return result.PageText;
            }
        }

        public string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
                var result = fulltextServiceClient.GetTextResourceFromSearch(textResource.ExternalId, format, searchRequest);
                return result.PageText;
            }
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
            
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
                var result = fulltextServiceClient.SearchByCriteriaCount(criteria);
                return result.Count;
            }
        }

        public FulltextSearchResultData SearchProjectIdByCriteria(int start, int count, SortTypeEnumContract? sort,
            SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);
            
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
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
        }

        public PageSearchResultData SearchPageByCriteria(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project)
        {
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
                var result = fulltextServiceClient.SearchPageByCriteria(project.SnapshotId, criteria);
                return new PageSearchResultData
                {
                    SearchResultType = PageSearchResultType.TextExternalId,
                    StringList = result.TextIdList,
                };
            }
        }

        public long SearchHitsResultCount(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project)
        {
            throw new System.NotImplementedException();
        }

        public SearchHitsResultData SearchHitsWithPageContext(int start, int count, int contextLength, List<SearchCriteriaContract> criteria,
            ProjectIdentificationResult project)
        {
            throw new System.NotImplementedException();
        }

        public long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
                var result = fulltextServiceClient.SearchCorpusByCriteriaCount(criteria);
                return result;
            }
        }

        public CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);
            
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
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
        }

        public long SearchHeadwordByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            throw new System.NotImplementedException();
        }

        public HeadwordSearchResultDataList SearchHeadwordByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
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