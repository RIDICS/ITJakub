using System.Collections.Generic;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Utils;
using Vokabular.Shared.DataContracts.Search.Criteria;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class SearchPageRequestContract
    {
        [JsonConverter(typeof(SearchCriteriaJsonConverter))]
        public IList<SearchCriteriaContract> ConditionConjunction { get; set; }
    }
}