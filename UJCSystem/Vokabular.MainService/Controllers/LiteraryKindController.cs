using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryKindController : Controller
    {
        private CatalogValueManager m_catalogValueManager;

        public LiteraryKindController(CatalogValueManager catalogValueManager)
        {
            m_catalogValueManager = catalogValueManager;
        }

        [HttpPost("")]
        public int CreateLiteraryKind([FromBody] LiteraryKindContract literaryKind)
        {
            var resultId = m_catalogValueManager.CreateLiteraryKind(literaryKind.Name);
            return resultId;
        }

        [HttpGet("")]
        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = m_catalogValueManager.GetLiteraryKindList();
            return result;
        }
    }
}