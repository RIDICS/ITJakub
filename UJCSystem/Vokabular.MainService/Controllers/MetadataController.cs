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
        public List<string> GetPublisherAutocomplete(string query)
        {
            var result = m_projectMetadataManager.GetPublisherAutocomplete(query);
            return result;
        }
    }
}
