using System;
using Microsoft.AspNetCore.Mvc;
using Vokabular.FulltextService.Core.Helpers.Converters;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Request;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class TextController : ApiControllerBase
    {
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

            try
            {
                result = m_textResourceManager.CreateTextResource(textResource);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(result);
        }

        [HttpPost("{textResourceId}/search")]
        public ActionResult<TextResourceContract> GetSearchTextResource(string textResourceId, [FromQuery] TextFormatEnumContract formatValue, [FromBody] SearchPageRequestContract searchPageRequestContract)
        {
            if (ContainsAnyUnsupportedCriteria(searchPageRequestContract))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var textResource = m_searchManager.SearchOnPageByCriteria(textResourceId, searchPageRequestContract);
            textResource.PageText = m_textConverter.Convert(textResource.PageText, formatValue);
            textResource.PageText = m_pageWithHtmlTagsCreator.CreatePage(textResource.PageText, formatValue);

            return textResource;
        }

        [HttpPost("snapshot/{snapshotId}/search")]
        public ActionResult<PageSearchResultContract> SearchPageByCriteria(long snapshotId, [FromBody] SearchPageRequestContract criteria)
        {
            if (ContainsAnyUnsupportedCriteria(criteria))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchPageByCriteria(snapshotId, criteria);
            return result;
        }

        [HttpPost("snapshot/{snapshotId}/search-hits-count")]
        public ActionResult<long> SearchHitsResultCount(long snapshotId, [FromBody] SearchPageRequestContract criteria)
        {
            if (ContainsAnyUnsupportedCriteria(criteria))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchHitsResultCount(snapshotId, criteria);
            return result;
        }

        [HttpPost("snapshot/{snapshotId}/search-context")]
        public ActionResult<HitsWithPageContextResultContract> SearchHitsWithPageContext(long snapshotId, [FromBody] SearchHitsRequestContract searchHitsRequestContract)
        {
            if (ContainsAnyUnsupportedCriteria(searchHitsRequestContract))
            {
                return BadRequest("Request contains unsupported criteria");
            }

            var result = m_searchManager.SearchHitsWithPageContext(snapshotId, searchHitsRequestContract);
            return result;
        }
    }
}