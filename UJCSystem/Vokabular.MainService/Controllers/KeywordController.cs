using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Utils.Documentation;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class KeywordController : BaseController
    {
        private readonly CatalogValueManager m_catalogValueManager;

        public KeywordController(CatalogValueManager catalogValueManager)
        {
            m_catalogValueManager = catalogValueManager;
        }

        [HttpPost("")]
        public int CreateKeyword([FromBody] KeywordContract data)
        {
            var resultId = m_catalogValueManager.CreateKeyword(data.Name);
            return resultId;
        }

        [HttpPut("{keywordId}")]
        public IActionResult UpdateKeyword(int keywordId, [FromBody] KeywordContract data)
        {
            try
            {
                m_catalogValueManager.UpdateKeyword(keywordId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("{keywordId}")]
        public IActionResult DeleteKeyword(int keywordId)
        {
            try
            {
                m_catalogValueManager.DeleteKeyword(keywordId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{keywordId}")]
        [ProducesResponseType(typeof(KeywordContract), StatusCodes.Status200OK)]
        public IActionResult GetKeyword(int keywordId)
        {
            var result = m_catalogValueManager.GetKeyword(keywordId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<KeywordContract> GetKeywordList([FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_catalogValueManager.GetKeywordList(start, count);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("autocomplete")]
        public List<KeywordContract> GetKeywordAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            var result = m_catalogValueManager.GetKeywordAutocomplete(query, count);
            return result;
        }
    }
}
