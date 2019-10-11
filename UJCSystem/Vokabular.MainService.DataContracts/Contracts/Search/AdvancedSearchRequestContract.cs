using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class AdvancedSearchRequestContract : SearchRequestContract
    {
        public SearchAdvancedParametersContract Parameters { get; set; }
    }
}