using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController : Controller
    {
        [HttpPost("")]
        public int CreateOriginalAuthor([FromBody] OriginalAuthorContract author)
        {
            throw new System.NotImplementedException();
        }
    }
}