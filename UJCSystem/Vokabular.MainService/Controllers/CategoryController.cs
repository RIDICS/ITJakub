using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
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

        [HttpGet("")]
        public List<CategoryContract> GetCategoryList()
        {
            return m_categoryManager.GetCategoryList();
        }
    }
}