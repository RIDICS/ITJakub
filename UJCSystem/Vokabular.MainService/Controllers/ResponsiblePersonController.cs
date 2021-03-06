﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ResponsiblePersonController : BaseController
    {
        private readonly PersonManager m_personManager;
        private readonly CatalogValueManager m_catalogValueManager;
        private readonly ProjectManager m_projectManager;

        public ResponsiblePersonController(PersonManager personManager, CatalogValueManager catalogValueManager, ProjectManager projectManager)
        {
            m_personManager = personManager;
            m_catalogValueManager = catalogValueManager;
            m_projectManager = projectManager;
        }

        [Authorize]
        [HttpPost("")]
        public int CreateResponsiblePerson([FromBody] ResponsiblePersonContract responsiblePerson)
        {
            return m_personManager.CreateResponsiblePerson(responsiblePerson);
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPut("{responsiblePersonId}")]
        public IActionResult UpdateResponsiblePerson(int responsiblePersonId, [FromBody] ResponsiblePersonContract data)
        {
            try
            {
                m_personManager.UpdateResponsiblePerson(responsiblePersonId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpDelete("{responsiblePersonId}")]
        public IActionResult DeleteResponsiblePerson(int responsiblePersonId)
        {
            try
            {
                m_personManager.DeleteResponsiblePerson(responsiblePersonId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{responsiblePersonId}")]
        [ProducesResponseType(typeof(ResponsiblePersonContract), StatusCodes.Status200OK)]
        public IActionResult GetResponsiblePerson(int responsiblePersonId)
        {
            var result = m_personManager.GetResponsiblePerson(responsiblePersonId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total records count")]
        public List<ResponsiblePersonContract> GetResponsiblePersonList([FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_personManager.GetResponsiblePersonList(start, count);
            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [HttpGet("autocomplete")]
        public List<ResponsiblePersonContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count)
        {
            return m_personManager.GetResponsiblePersonAutocomplete(query, count);
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPost("type")]
        public int CreateResponsibleType([FromBody] ResponsibleTypeContract responsibleType)
        {
            return m_catalogValueManager.CreateResponsibleType(responsibleType);
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
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

        [Authorize(VokabularPermissionNames.ManageCodeList)]
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

        [HttpGet("{responsiblePersonId}/project")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<ProjectDetailContract> GetProjectsByResponsiblePerson(int responsiblePersonId, [FromQuery] int? start, [FromQuery] int? count)
        {
            var result = m_projectManager.GetProjectsByResponsiblePerson(responsiblePersonId, start, count);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }
    }
}