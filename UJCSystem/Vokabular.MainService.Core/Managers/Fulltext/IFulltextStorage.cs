using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public interface IFulltextStorage
    {
        ProjectType ProjectType { get; }
        string GetPageText(TextResource textResource, TextFormatEnumContract format);
        string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format, SearchPageRequestContract searchRequest);
        string GetHeadwordText(HeadwordResource headwordResource, TextFormatEnumContract format);
        string GetHeadwordTextFromSearch(HeadwordResource headwordResource, TextFormatEnumContract format, SearchPageRequestContract searchRequest);
        long SearchByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        FulltextSearchResultData SearchProjectIdByCriteria(int start, int count, SortTypeEnumContract? sort, SortDirectionEnumContract? sortDirection, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        PageSearchResultData SearchPageByCriteria(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project);
        long SearchHitsResultCount(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project);
        SearchHitsResultData SearchHitsWithPageContext(int start, int count, int contextLength, List<SearchCriteriaContract> criteria, ProjectIdentificationResult project);
        long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        long SearchHeadwordByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        HeadwordSearchResultDataList SearchHeadwordByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        string GetEditionNote(EditionNoteResource editionNoteResource, TextFormatEnumContract format);
        string CreateNewTextVersion(TextResource textResource);
        string CreateNewHeadwordVersion(HeadwordResource headwordResource);
        string CreateNewEditionNoteVersion(EditionNoteResource editionNoteResource);
        CorpusSearchSnapshotsResultContract SearchCorpusGetSnapshotListByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects, bool fetchNumberOfresults);
        CorpusSearchResultDataList SearchCorpusInSnapshotByCriteria(long projectId, int start, int count, int contextLength, List<SearchCriteriaContract> criteria);
        long SearchCorpusTotalResultCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
    }
}
