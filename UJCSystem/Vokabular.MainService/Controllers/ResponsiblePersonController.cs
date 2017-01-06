using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ResponsiblePersonController : Controller
    {
        [HttpPost("")]
        public int CreateResponsiblePerson([FromBody] ResponsiblePersonContract responsiblePerson)
        {
            throw new System.NotImplementedException();
        }
    }
}