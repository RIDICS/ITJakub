using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class HeadwordController : Controller
    {
        private readonly BookManager m_bookManager;
        private readonly BookSearchManager m_bookSearchManager;

        public HeadwordController(BookManager bookManager, BookSearchManager bookSearchManager)
        {
            m_bookManager = bookManager;
            m_bookSearchManager = bookSearchManager;
        }

        [HttpPost("search")]
        public List<HeadwordContract> SearchHeadword([FromBody] HeadwordSearchRequestContract request)
        {
            var result = m_bookSearchManager.SearchHeadwordByCriteria(request);
            return result;
        }

        [HttpPost("search-count")]
        public long SearchHeadwordResultCount([FromBody] HeadwordSearchRequestContract request)
        {
            var result = m_bookSearchManager.SearchHeadwordByCriteriaCount(request);
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