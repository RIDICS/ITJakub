using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Search;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class CorpusController : Controller
    {
        private readonly BookSearchManager m_bookSearchManager;

        public CorpusController(BookSearchManager bookSearchManager)
        {
            m_bookSearchManager = bookSearchManager;
        }

        [HttpPost("search")]
        public List<CorpusSearchResultContract> SearchCorpus([FromBody] CorpusSearchRequestContract request)
        {
            var result = m_bookSearchManager.SearchCorpusByCriteria(request);
            return result;
        }

        [HttpPost("search-count")]
        public long SearchCorpusResultCount([FromBody] CorpusSearchRequestContract request)
        {
            var result = m_bookSearchManager.SearchCorpusByCriteriaCount(request);
            return result;
        }
    }
}