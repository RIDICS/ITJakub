using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController : Controller
    {
        private readonly PersonManager m_personManager;

        public AuthorController(PersonManager personManager)
        {
            m_personManager = personManager;
        }

        [HttpPost("")]
        public int CreateOriginalAuthor([FromBody] OriginalAuthorContract author)
        {
            return m_personManager.CreateOriginalAuthor(author);
        }
    }
}