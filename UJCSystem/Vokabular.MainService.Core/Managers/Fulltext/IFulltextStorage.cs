using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.MainService.Core.Managers.Fulltext.Data;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Search.Criteria;

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
        FulltextSearchResultData SearchProjectIdByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        PageSearchResultData SearchPageByCriteria(List<SearchCriteriaContract> criteria, ProjectIdentificationResult project);
        long SearchCorpusByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        CorpusSearchResultDataList SearchCorpusByCriteria(int start, int count, int contextLength, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        long SearchHeadwordByCriteriaCount(List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        HeadwordSearchResultDataList SearchHeadwordByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects);
        string GetEditionNote(ProjectIdentificationResult project, TextFormatEnumContract format);
    }
}
