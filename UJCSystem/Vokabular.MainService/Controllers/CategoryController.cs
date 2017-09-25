using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Clients.Errors;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly CategoryManager m_categoryManager;

        public CategoryController(CategoryManager categoryManager)
        {
            m_categoryManager = categoryManager;
        }

        [HttpPost("")]
        public int CreateCategory([FromBody] CategoryContract category)
        {
            return m_categoryManager.CreateCategory(category);
        }

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

        [HttpGet("")]
        public List<CategoryContract> GetCategoryList()
        {
            return m_categoryManager.GetCategoryList();
        }
    }
}