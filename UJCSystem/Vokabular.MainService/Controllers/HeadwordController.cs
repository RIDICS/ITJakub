using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class HeadwordController : Controller
    {
        private readonly BookManager m_bookManager;

        public HeadwordController(BookManager bookManager)
        {
            m_bookManager = bookManager;
        }

        [HttpPost("search")]
        public List<HeadwordContract> SearchHeadword([FromBody] HeadwordSearchRequestContract request)
        {
            var result = m_bookManager.SearchHeadwordByCriteria(request);
            return result;
        }

        [HttpPost("search-count")]
        public long SearchHeadwordResultCount([FromBody] HeadwordSearchRequestContract request)
        {
            var result = m_bookManager.SearchHeadwordByCriteriaCount(request);
            return result;
        }

        [HttpPost("search-row-number")]
        public long SearchHeadwordRowNumber([FromBody] HeadwordRowNumberSearchRequestContract request)
        {
            var result = m_bookManager.SearchHeadwordRowNumber(request);
            return result;
        }

        [HttpGet("autocomplete")]
        public List<string> GetAutocomplete([FromQuery] string query, BookTypeEnumContract? bookType = null, IList<int> selectedCategoryIds = null, IList<long> selectedProjectIds = null)
        {
            return m_bookManager.GetHeadwordAutocomplete(query, bookType, selectedCategoryIds, selectedProjectIds);
        }
    }
}