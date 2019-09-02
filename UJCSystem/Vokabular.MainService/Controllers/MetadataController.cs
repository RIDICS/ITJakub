using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class MetadataController : BaseController
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

        [HttpGet("title/autocomplete")]
        public List<string> GetTitleAutocomplete([FromQuery] string query, [FromQuery] BookTypeEnumContract? bookType,
            [FromQuery] List<int> selectedCategoryIds, [FromQuery] List<long> selectedProjectIds)
        {
            var result = m_projectMetadataManager.GetTitleAutocomplete(query, bookType, selectedCategoryIds, selectedProjectIds);
            return result;
        }
    }
}
