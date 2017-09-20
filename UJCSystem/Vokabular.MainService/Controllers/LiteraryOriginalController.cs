using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryOriginalController : Controller
    {
        private readonly ProjectMetadataManager m_projectMetadataManager;

        public LiteraryOriginalController(ProjectMetadataManager projectMetadataManager)
        {
            m_projectMetadataManager = projectMetadataManager;
        }

        [HttpGet("")]
        public List<LiteraryOriginalContract> GetLiteraryOriginalList()
        {
            var result = m_projectMetadataManager.GetLiteraryOriginalList();
            return result;
        }
    }
}