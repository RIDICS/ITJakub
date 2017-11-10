using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.SelectResults;
using Vokabular.MainService.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.Shared.DataContracts.Search.Criteria;

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
            throw new System.NotImplementedException();
        }

        public string GetPageTextFromSearch(TextResource textResource, TextFormatEnumContract format,
            SearchPageRequestContract searchRequest)
        {
            throw new System.NotImplementedException();
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

            throw new System.NotImplementedException();
        }

        public List<long> SearchProjectIdByCriteria(int start, int count, List<SearchCriteriaContract> criteria, IList<ProjectIdentificationResult> projects)
        {
            UpdateCriteriaWithSnapshotRestriction(criteria, projects);

            FulltextSearchResultContract result;
            using (var fulltextServiceClient = m_communicationProvider.GetFulltextServiceClient())
            {
                result = fulltextServiceClient.SearchByCriteria(criteria);
            }

            return result.ProjectIds;
        }
    }
}