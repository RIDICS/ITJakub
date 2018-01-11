using System.Collections.Generic;
using Newtonsoft.Json;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Utils;

namespace Vokabular.Shared.DataContracts.Search.Request
{
    public class SearchPageRequestContract
    {
        [JsonConverter(typeof(SearchCriteriaJsonConverter))]
        public List<SearchCriteriaContract> ConditionConjunction { get; set; }
    }
}