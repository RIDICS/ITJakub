using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;

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
    }
}