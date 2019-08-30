using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectEditionNoteController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;

        public ProjectEditionNoteController(ProjectItemManager projectItemManager)
        {
            m_projectItemManager = projectItemManager;
        }

        [HttpGet("{projectId}/edition-note")]
        [ProducesResponseType(typeof(EditionNoteContract), StatusCodes.Status200OK)]
        public IActionResult GetEditionNote(long projectId, TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_projectItemManager.GetEditionNote(projectId, formatValue);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{projectId}/edition-note")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateEditionNoteVersion(long projectId, [FromBody] CreateEditionNoteContract data)
        {
            try
            {
                var resultId = m_projectItemManager.CreateEditionNoteVersion(projectId, data);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }
    }
}