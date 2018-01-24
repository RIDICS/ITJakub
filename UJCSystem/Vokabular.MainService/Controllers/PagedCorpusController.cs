using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Search;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Search.Corpus;
using Vokabular.Shared.DataContracts.Search.Request;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class PagedCorpusController : Controller
    {
        private readonly BookSearchManager m_bookSearchManager;

        public PagedCorpusController(BookSearchManager bookSearchManager)
        {
            m_bookSearchManager = bookSearchManager;
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(CorpusSearchSnapshotsResultContract), StatusCodes.Status200OK)]
        public IActionResult SearchCorpusSnapshotsResult([FromBody] CorpusSearchRequestContract request)
        {
            try
            {
                var result = m_bookSearchManager.SearchCorpusSnapshotsByCriteria(request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpPost("{snapshotId}/search")]
        [ProducesResponseType(typeof(List<CorpusSearchResultContract>), StatusCodes.Status200OK)]
        public IActionResult SearchCorpusSnapshotResult(long snapshotId, [FromBody] CorpusSearchRequestContract request)
        {
            try
            {
                var result = m_bookSearchManager.SearchCorpusSnapshotByCriteria(snapshotId, request);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

    }
}