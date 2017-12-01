using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectEditionNoteController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;

        public ProjectEditionNoteController(ProjectItemManager projectItemManager)
        {
            m_projectItemManager = projectItemManager;
        }

        [HttpGet("{projectId}")]
        public EditionNoteContract GetEditionNote(long projectId, TextFormatEnumContract? format)
        {
            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_projectItemManager.GetEditionNote(projectId, formatValue);
            return result;
        }

        [HttpPost("{projectId}")]
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