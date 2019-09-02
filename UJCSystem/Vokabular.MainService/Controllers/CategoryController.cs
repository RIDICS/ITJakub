using System.Collections.Generic;
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
    public class CategoryController : BaseController
    {
        private readonly CategoryManager m_categoryManager;

        public CategoryController(CategoryManager categoryManager)
        {
            m_categoryManager = categoryManager;
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPost("")]
        public int CreateCategory([FromBody] CategoryContract category)
        {
            return m_categoryManager.CreateCategory(category);
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpPut("{categoryId}")]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryContract category)
        {
            try
            {
                m_categoryManager.UpdateCategory(categoryId, category);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int)exception.StatusCode, exception.Message);
            }
        }

        [Authorize(VokabularPermissionNames.ManageCodeList)]
        [HttpDelete("{categoryId}")]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                m_categoryManager.DeleteCategory(categoryId);
                return Ok();
            }
            catch (HttpErrorCodeException exception)
            {
                return StatusCode((int) exception.StatusCode, exception.Message);
            }
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(typeof(CategoryContract), StatusCodes.Status200OK)]
        public IActionResult GetCategory(int categoryId)
        {
            var result = m_categoryManager.GetCategory(categoryId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("")]
        public List<CategoryContract> GetCategoryList()
        {
            return m_categoryManager.GetCategoryList();
        }

        [HttpGet("tree")]
        public List<CategoryTreeContract> GetCategoryTree()
        {
            var result = m_categoryManager.GetCategoryTree();
            return result;
        }
    }
}