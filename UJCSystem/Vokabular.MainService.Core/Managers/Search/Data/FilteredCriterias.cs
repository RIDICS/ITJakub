using System.Collections.Generic;
using Vokabular.DataEntities.Database.QueryBuilder;
using Vokabular.Shared.DataContracts.Search.Criteria;

namespace Vokabular.MainService.Core.Managers.Search.Data
{
    public class FilteredCriterias
    {
        public List<SearchCriteriaQuery> ConjunctionQuery { get; set; }
        public List<SearchCriteriaContract> NonMetadataCriterias { get; set; }
        public List<SearchCriteriaContract> MetadataCriterias { get; set; }
        public Dictionary<string, object> MetadataParameters { get; set; }
    }
}
