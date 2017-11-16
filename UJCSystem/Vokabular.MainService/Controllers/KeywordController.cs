using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class KeywordController : Controller
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
        public List<KeywordContract> GetKeywordList()
        {
            var result = m_catalogValueManager.GetKeywordList();
            return result;
        }
    }
}
