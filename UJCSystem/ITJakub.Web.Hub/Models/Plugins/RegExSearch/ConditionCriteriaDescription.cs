using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    public class ConditionCriteriaDescription
    {
        public IList<WordCriteriaDescription> WordCriteriaDescription { get; set; }
        public int SearchType { get; set; }
    }
}