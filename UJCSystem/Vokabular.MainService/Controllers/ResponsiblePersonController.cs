using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ResponsiblePersonController : Controller
    {
        private readonly PersonManager m_personManager;
        private readonly CatalogValueManager m_catalogValueManager;

        public ResponsiblePersonController(PersonManager personManager, CatalogValueManager catalogValueManager)
        {
            m_personManager = personManager;
            m_catalogValueManager = catalogValueManager;
        }

        [HttpPost("")]
        public int CreateResponsiblePerson([FromBody] ResponsiblePersonContract responsiblePerson)
        {
            return m_personManager.CreateResponsiblePerson(responsiblePerson);
        }

        [HttpGet("autocomplete")]
        public List<ResponsiblePersonContract> GetAutocomplete([FromQuery] string query)
        {
            return m_personManager.GetResponsiblePersonAutocomplete(query);
        }

        [HttpPost("type")]
        public int CreateResponsibleType([FromBody] ResponsibleTypeContract responsibleType)
        {
            return m_catalogValueManager.CreateResponsibleType(responsibleType);
        }

        [HttpPut("type/{responsibleTypeId}")]
        public IActionResult UpdateResponsibleType(int responsibleTypeId, [FromBody] ResponsibleTypeContract data)
        {
            try
            {
                m_catalogValueManager.UpdateResponsibleType(responsibleTypeId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpDelete("type/{responsibleTypeId}")]
        public IActionResult DeleteResponsibleType(int responsibleTypeId)
        {
            try
            {
                m_catalogValueManager.DeleteResponsibleType(responsibleTypeId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("type/{responsibleTypeId}")]
        [ProducesResponseType(typeof(ResponsibleTypeContract), StatusCodes.Status200OK)]
        public IActionResult GetResponsibleType(int responsibleTypeId)
        {
            var result = m_catalogValueManager.GetResponsibleType(responsibleTypeId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        
        [HttpGet("type")]
        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            return m_catalogValueManager.GetResponsibleTypeList();
        }
    }
}