using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Shared.DataContracts.Search.QueryBuilder
{
    public class SearchCriteriaQuery
    {
        public CriteriaKey CriteriaKey { get; set; }

        public string Join { get; set; }
        
        public string Where { get; set; }

        public object Restriction { get; set; } // TODO change to specific type after DB migration to VokabularDB
    }
}