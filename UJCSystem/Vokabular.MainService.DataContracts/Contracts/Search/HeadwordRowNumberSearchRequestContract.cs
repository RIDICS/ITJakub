using Vokabular.Shared.DataContracts.Search.Criteria;

namespace Vokabular.MainService.DataContracts.Contracts.Search
{
    public class HeadwordRowNumberSearchRequestContract
    {
        public string Query { get; set; }

        public SelectedCategoryCriteriaContract Category { get; set; }
    }
}