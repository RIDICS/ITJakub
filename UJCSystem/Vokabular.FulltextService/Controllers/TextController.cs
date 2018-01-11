using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class TextController : Controller
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<TextController>();

        private readonly TextResourceManager m_textResourceManager;
        private readonly ITextConverter m_textConverter;
        private readonly SearchManager m_searchManager;
        public TextController(TextResourceManager textResourceManager, ITextConverter textConverter, SearchManager searchManager)
        {
            m_textResourceManager = textResourceManager;
            m_textConverter = textConverter;
            m_searchManager = searchManager;
        }

        [HttpGet("{textResourceId}")]
        public TextResourceContract GetTextResource(string textResourceId, [FromQuery] TextFormatEnumContract formatValue)
        {
            var textResource = m_textResourceManager.GetTextResource(textResourceId);
            textResource.PageText = m_textConverter.Convert(textResource.PageText, formatValue);
            
            return textResource;
        }

        [HttpPost]
        public ResultContract CreateTextResource([FromBody] TextResourceContract textResource)
        {
            var result = m_textResourceManager.CreateTextResource(textResource);
            return result;
        }

        [HttpPost("search/{textResourceId}")]
        public TextResourceContract GetSearchTextResource(string textResourceId, [FromQuery] TextFormatEnumContract formatValue, [FromBody] SearchPageRequestContract searchPageRequestContract )
        {
            var textResource = m_searchManager.SearchPageByCriteria(textResourceId, searchPageRequestContract);
            textResource.PageText = m_textConverter.Convert(textResource.PageText, formatValue);

            return textResource;
        }

        [HttpPost("search")]
        public PageSearchResultContract SearchPageByCriteria([FromQuery] long snapshotId, [FromBody] SearchRequestContractBase criteria)
        {
            var result = m_searchManager.SearchPageByCriteria(snapshotId, criteria);
            return result;
        }
    }
}