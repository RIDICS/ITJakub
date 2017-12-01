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
                }
            }

            return new FilteredCriterias
            {
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

            if (countDictionary[CriteriaKey.Result] > 0)
                throw new ArgumentException("Result criteria is not allowed.");

            if (countDictionary[CriteriaKey.ResultRestriction] > 0)
                throw new ArgumentException("ResultRestriction criteria is not allowed.");

            if (countDictionary[CriteriaKey.SnapshotResultRestriction] > 0)
                throw new ArgumentException("SnapshotResultRestriction criteria is not allowed.");

            if (countDictionary[CriteriaKey.SelectedCategory] > 1)
                throw new ArgumentException("Only one SelectedCategory criteria is allowed.");
        }
    }
}
