using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryOriginalController : Controller
    {
        private readonly CatalogValueManager m_catalogValueManager;

        public LiteraryOriginalController(CatalogValueManager catalogValueManager)
        {
            m_catalogValueManager = catalogValueManager;
        }

        [HttpGet("")]
        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            var result = m_catalogValueManager.GetLiteraryOriginalList();
            return result;
        }
    }
}