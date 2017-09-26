using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class MetadataController : Controller
    {
        private readonly ProjectMetadataManager m_projectMetadataManager;

        public MetadataController(ProjectMetadataManager projectMetadataManager)
        {
            m_projectMetadataManager = projectMetadataManager;
        }

        [HttpGet("publisher/autocomplete")]
        public List<string> GetPublisherAutocomplete([FromQuery] string query)
        {
            var result = m_projectMetadataManager.GetPublisherAutocomplete(query);
            return result;
        }

        [HttpGet("copyright/autocomplete")]
        public List<string> GetCopyrightAutocomplete([FromQuery] string query)
        {
            var result = m_projectMetadataManager.GetCopyrightAutocomplete(query);
            return result;
        }

        [HttpGet("manuscript/repository/autocomplete")]
        public List<string> GetManuscriptRepositoryAutocomplete([FromQuery] string query)
        {
            var result = m_projectMetadataManager.GetManuscriptRepositoryAutocomplete(query);
            return result;
        }
    }
}
