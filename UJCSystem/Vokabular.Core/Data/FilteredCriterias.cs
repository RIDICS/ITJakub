using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Search.QueryBuilder;

namespace Vokabular.Core.Data
{
    public class FilteredCriterias
    {
        public List<SearchCriteriaQuery> ConjunctionQuery { get; set; }
        public List<SearchCriteriaContract> NonMetadataCriterias { get; set; }
        public List<SearchCriteriaContract> MetadataCriterias { get; set; }
        public Dictionary<string, object> MetadataParameters { get; set; }
        public ResultCriteriaContract ResultCriteria { get; set; } // TODO probably remove this property
    }
}
