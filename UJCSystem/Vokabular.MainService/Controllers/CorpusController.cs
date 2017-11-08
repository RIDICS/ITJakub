using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class CorpusController : Controller
    {
        private readonly BookManager m_bookManager;

        public CorpusController(BookManager bookManager)
        {
            m_bookManager = bookManager;
        }

        [HttpPost("search")]
        public List<CorpusSearchResultContract> SearchCorpus([FromBody] CorpusSearchRequestContract request)
        {
            var result = m_bookManager.SearchCorpusByCriteria(request);
            return result;
        }

        [HttpPost("search-count")]
        public long SearchCorpusResultCount([FromBody] CorpusSearchRequestContract request)
        {
            var result = m_bookManager.SearchCorpusByCriteriaCount(request);
            return result;
        }
    }
}