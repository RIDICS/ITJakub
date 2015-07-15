using System.Collections.Generic;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    public class WordListCriteriaDescription : ConditionCriteriaDescriptionBase
    {
        public IList<WordCriteriaDescription> Disjunctions { get; set; }
    }
}