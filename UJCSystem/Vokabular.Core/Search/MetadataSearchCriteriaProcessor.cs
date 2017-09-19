using System;
using System.Collections.Generic;
using System.Linq;
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
            ValidateSearchCriterias(searchCriterias);

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

        private void ValidateSearchCriterias(IList<SearchCriteriaContract> searchCriterias)
        {
            var countDictionary = Enum.GetValues(typeof(CriteriaKey))
                .Cast<CriteriaKey>()
                .ToDictionary(criteriaKey => criteriaKey, criteriaKey => 0);

            foreach (var searchCriteriaContract in searchCriterias)
            {
                countDictionary[searchCriteriaContract.Key]++;
            }

            if (countDictionary[CriteriaKey.Authorization] > 1)
                throw new ArgumentException("Only one Authorization criteria is allowed.");

            if (countDictionary[CriteriaKey.Result] > 1)
                throw new ArgumentException("Only one Result criteria is allowed.");

            if (countDictionary[CriteriaKey.ResultRestriction] > 1)
                throw new ArgumentException("Only one ResultRestriction criteria is allowed.");

            if (countDictionary[CriteriaKey.SelectedCategory] > 1)
                throw new ArgumentException("Only one SelectedCategory criteria is allowed.");
        }
    }
}
