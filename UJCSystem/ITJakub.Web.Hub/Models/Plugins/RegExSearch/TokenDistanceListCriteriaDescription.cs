using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    public class TokenDistanceListCriteriaDescription : ConditionCriteriaDescriptionBase
    {
        public IList<TokenDistanceCriteriaDescription> Disjunctions { get; set; }
    }
}