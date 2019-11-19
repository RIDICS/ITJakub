using NHibernate.Criterion;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.DataEntities.Database.QueryBuilder
{
    public class SearchCriteriaQuery
    {
        public CriteriaKey CriteriaKey { get; set; }

        public string Join { get; set; }
        
        public string Where { get; set; }

        public ICriterion Restriction { get; set; }
    }
}