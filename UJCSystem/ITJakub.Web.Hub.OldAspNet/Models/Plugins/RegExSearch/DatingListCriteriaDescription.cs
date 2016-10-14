using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    public class DatingListCriteriaDescription : ConditionCriteriaDescriptionBase
    {
        public IList<DatingCriteriaDescription> Disjunctions { get; set; }
    }
}