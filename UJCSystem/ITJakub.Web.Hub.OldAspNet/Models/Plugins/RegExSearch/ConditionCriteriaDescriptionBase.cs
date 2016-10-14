using ITJakub.Web.Hub.Converters;
using Newtonsoft.Json;

namespace ITJakub.Web.Hub.Models.Plugins.RegExSearch
{
    [JsonConverter(typeof (ConditionCriteriaDescriptionConverter))]
    public class ConditionCriteriaDescriptionBase
    {
        public int SearchType { get; set; }
        public ConditionTypeEnum ConditionType { get; set; }
    }
}