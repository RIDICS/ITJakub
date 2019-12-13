﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class TermController : BaseController
    {
        private readonly TermManager m_termManager;
        private readonly CatalogValueManager m_catalogValueManager;

        public TermController(TermManager termManager, CatalogValueManager catalogValueManager)
        {
            m_termManager = termManager;
            m_catalogValueManager = catalogValueManager;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPost("")]
        public int CreateTerm([FromBody] TermContract term)
        {
            return m_termManager.CreateTerm(term);
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPut("{termId}")]
        public IActionResult UpdateTerm(int termId, [FromBody] TermContract data)
        {
            try
            {
                m_termManager.UpdateTerm(termId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpDelete("{termId}")]
        public IActionResult DeleteTerm(int termId)
        {
            try
            {
                m_termManager.DeleteTerm(termId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{termId}")]
        [ProducesResponseType(typeof(TermContract), StatusCodes.Status200OK)]
        public IActionResult GetTerm(int termId)
        {
            var result = m_termManager.GetTerm(termId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        public List<TermContract> GetTermList([FromQuery] int? termCategoryId)
        {
            var result = m_termManager.GetTermList(termCategoryId);
            return result;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPost("category")]
        public int CreateTermCategory([FromBody] TermCategoryContract termCategory)
        {
            return m_catalogValueManager.CreateTermCategory(termCategory);
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPut("category/{termCategoryId}")]
        public IActionResult UpdateTermCategory(int termCategoryId, [FromBody] TermCategoryContract data)
        {
            try
            {
                m_catalogValueManager.UpdateTermCategory(termCategoryId, data);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpDelete("category/{termCategoryId}")]
        public IActionResult DeleteTermCategory(int termCategoryId)
        {
            try
            {
                m_catalogValueManager.DeleteTermCategory(termCategoryId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("category/{termCategoryId}")]
        [ProducesResponseType(typeof(TermCategoryContract), StatusCodes.Status200OK)]
        public IActionResult GetTermCategory(int termCategoryId)
        {
            var result = m_catalogValueManager.GetTermCategory(termCategoryId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("category")]
        public List<TermCategoryContract> GetTermCategoryList()
        {
            return m_catalogValueManager.GetTermCategoryList();
        }

        [HttpGet("category/detail")]
        public List<TermCategoryDetailContract> GetTermCategoriesWithTerms()
        {
            return m_catalogValueManager.GetTermCategoriesWithTerms();
        }
    }
}