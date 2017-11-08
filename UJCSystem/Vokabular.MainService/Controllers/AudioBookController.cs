using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.Shared.DataContracts.Search;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class AudioBookController : Controller
    {
        private readonly BookManager m_bookManager;

        public AudioBookController(BookManager bookManager)
        {
            m_bookManager = bookManager;
        }

        [HttpPost("search")]
        public List<AudioBookSearchResultContract> SearchBook([FromBody] SearchRequestContract request)
        {
            var result = m_bookManager.SearchAudioByCriteria(request);
            return result;
        }

        [HttpPost("search-count")]
        public long SearchBookResultCount([FromBody] SearchRequestContract request)
        {
            var result = m_bookManager.SearchByCriteriaCount(request);
            return result;
        }

        [HttpGet("{projectId}")]
        public AudioBookSearchResultContract GetBookDetail(long projectId)
        {
            var result = m_bookManager.GetAudioBookDetail(projectId);
            return result;
        }
    }
}