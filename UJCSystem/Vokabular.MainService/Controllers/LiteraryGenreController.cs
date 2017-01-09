using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class LiteraryGenreController : Controller
    {
        private readonly ProjectMetadataManager m_projectMetadataManager;

        public LiteraryGenreController(ProjectMetadataManager projectMetadataManager)
        {
            m_projectMetadataManager = projectMetadataManager;
        }

        [HttpPost("")]
        public int CreateLiteraryGenre([FromBody] LiteraryGenreContract literaryGenre)
        {
            var resultId = m_projectMetadataManager.CreateLiteraryGenre(literaryGenre.Name);
            return resultId;
        }

        [HttpGet("")]
        public List<LiteraryGenreContract> GetLiteraryGenreList()
        {
            var result = m_projectMetadataManager.GetLiteraryGenreList();
            return result;
        }
    }
}