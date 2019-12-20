using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectEditionNoteController : BaseController
    {
        private readonly EditionNoteManager m_editionNoteManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectEditionNoteController(EditionNoteManager editionNoteManager, AuthorizationManager authorizationManager)
        {
            m_editionNoteManager = editionNoteManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpGet("{projectId}/edition-note")]
        [ProducesResponseType(typeof(EditionNoteContract), StatusCodes.Status200OK)]
        public IActionResult GetLatestEditionNote(long projectId, TextFormatEnumContract? format)
        {
            // Authorized in EditionNoteManager

            var formatValue = format ?? TextFormatEnumContract.Html;
            var result = m_editionNoteManager.GetLatestEditionNote(projectId, formatValue);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{projectId}/edition-note")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateEditionNoteVersion(long projectId, [FromBody] CreateEditionNoteContract data)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            try
            {
                var resultId = m_editionNoteManager.CreateEditionNoteVersion(projectId, data);
                return Ok(resultId);
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }
    }
}