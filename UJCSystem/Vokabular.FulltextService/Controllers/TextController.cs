using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Helpers.Converters;
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
        private readonly IPageWithHtmlTagsCreator m_pageWithHtmlTagsCreator;

        public TextController(TextResourceManager textResourceManager, ITextConverter textConverter, SearchManager searchManager, IPageWithHtmlTagsCreator pageWithHtmlTagsCreator)
        {
            m_textResourceManager = textResourceManager;
            m_textConverter = textConverter;
            m_searchManager = searchManager;
            m_pageWithHtmlTagsCreator = pageWithHtmlTagsCreator;
        }

        [HttpGet("{textResourceId}")]
        public TextResourceContract GetTextResource(string textResourceId, [FromQuery] TextFormatEnumContract formatValue)
        {
            var textResource = m_textResourceManager.GetTextResource(textResourceId);
            textResource.PageText = m_textConverter.Convert(textResource.PageText, formatValue);
            textResource.PageText = m_pageWithHtmlTagsCreator.CreatePage(textResource.PageText, formatValue);

            return textResource;
        }

        [HttpPost]
        public ActionResult CreateTextResource([FromBody] TextResourceContract textResource)
        {
            ResultContract result;

            try{
                result = m_textResourceManager.CreateTextResource(textResource);
            }catch (ArgumentException exception){
                return BadRequest(exception.Message);
            }

            return Ok(result);
        }

        [HttpPost("{textResourceId}/search")]
        public TextResourceContract GetSearchTextResource(string textResourceId, [FromQuery] TextFormatEnumContract formatValue, [FromBody] SearchPageRequestContract searchPageRequestContract )
        {
            var textResource = m_searchManager.SearchPageByCriteria(textResourceId, searchPageRequestContract);
            textResource.PageText = m_textConverter.Convert(textResource.PageText, formatValue);
            textResource.PageText = m_pageWithHtmlTagsCreator.CreatePage(textResource.PageText, formatValue);

            return textResource;
        }

        [HttpPost("snapshot/{snapshotId}/search")]
        public PageSearchResultContract SearchPageByCriteria(long snapshotId, [FromBody] SearchRequestContractBase criteria)
        {
            var result = m_searchManager.SearchPageByCriteria(snapshotId, criteria);
            return result;
        }

        [HttpPost("snapshot/{snapshotId}/search-count")]
        public long SearchPageByCriteriaCount(long snapshotId, [FromBody] SearchRequestContractBase criteria)
        {
            var result = m_searchManager.SearchPageByCriteriaCount(snapshotId, criteria);
            return result;
        }
    }
}