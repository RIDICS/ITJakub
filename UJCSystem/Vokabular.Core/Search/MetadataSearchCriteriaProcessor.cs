using System.Collections.Generic;
using Vokabular.Core.Data;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Search
{
    public class MetadataSearchCriteriaProcessor
    {
        private readonly MetadataSearchCriteriaDirector m_searchCriteriaDirector;

        public MetadataSearchCriteriaProcessor(MetadataSearchCriteriaDirector searchCriteriaDirector)
        {
            m_searchCriteriaDirector = searchCriteriaDirector;
        }

        public FilteredCriterias ProcessSearchCriterias(IList<SearchCriteriaContract> searchCriterias)
        {
            ResultCriteriaContract resultCriteria = null;
            var nonMetadataCriterias = new List<SearchCriteriaContract>();
            var metadataCriterias = new List<SearchCriteriaContract>();
            var conjunction = new List<SearchCriteriaQuery>();
            var metadataParameters = new Dictionary<string, object>();

            foreach (var searchCriteriaContract in searchCriterias)
            {
                if (m_searchCriteriaDirector.IsCriteriaSupported(searchCriteriaContract))
                {
                    var criteriaQuery = m_searchCriteriaDirector.ProcessCriteria(searchCriteriaContract,
                        metadataParameters);
                    conjunction.Add(criteriaQuery);
                    metadataCriterias.Add(searchCriteriaContract);
                }
                else
                {
                    nonMetadataCriterias.Add(searchCriteriaContract);

                    if (searchCriteriaContract.Key == CriteriaKey.Result)
                        resultCriteria = (ResultCriteriaContract)searchCriteriaContract;
                }
            }

            return new FilteredCriterias
            {
                ResultCriteria = resultCriteria,
                NonMetadataCriterias = nonMetadataCriterias,
                MetadataCriterias = metadataCriterias,
                MetadataParameters = metadataParameters,
                ConjunctionQuery = conjunction
            };
        }
    }
}
