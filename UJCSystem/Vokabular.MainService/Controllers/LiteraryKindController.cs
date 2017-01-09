using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryKindController : Controller
    {
        private readonly ProjectMetadataManager m_projectMetadataManager;

        public LiteraryKindController(ProjectMetadataManager projectMetadataManager)
        {
            m_projectMetadataManager = projectMetadataManager;
        }

        [HttpPost("")]
        public int CreateLiteraryKind([FromBody] LiteraryKindContract literaryKind)
        {
            var resultId = m_projectMetadataManager.CreateLiteraryKind(literaryKind.Name);
            return resultId;
        }

        [HttpGet("")]
        public List<LiteraryKindContract> GetLiteraryKindList()
        {
            var result = m_projectMetadataManager.GetLiteraryKindList();
            return result;
        }
    }
}