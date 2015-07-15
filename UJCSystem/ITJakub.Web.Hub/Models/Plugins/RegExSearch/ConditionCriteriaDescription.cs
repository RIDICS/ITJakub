using System.Collections.Generic;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    [JsonConverter(typeof(ConditionCriteriaDescriptionConverter))]
    public class ConditionCriteriaDescription
    {
        public int SearchType { get; set; }

        public ConditionTypeEnum ConditionType { get; set; }

    }

    public class WordListCriteriaDescription : ConditionCriteriaDescription
    {
        public IList<WordCriteriaDescription> WordCriteriaDescription { get; set; }
    }

    public class WordCriteriaDescription
    {
        public string StartsWith { get; set; }
        public IList<string> Contains { get; set; }
        public string EndsWith { get; set; }
    }


    public class DatingListCriteriaDescription : ConditionCriteriaDescription
    {
        public IList<DatingCriteriaDescription> DatingCriteriaDescription { get; set; }
    }

    public class DatingCriteriaDescription
    {
        public int? NotAfter { get; set; }
        public int? NotBefore { get; set; }
    }

    public enum ConditionTypeEnum
    {
        WordList = 0,
        DatingList = 1
    }
}