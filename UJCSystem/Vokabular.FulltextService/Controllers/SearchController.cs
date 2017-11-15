using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Search;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<SearchController>();

        private readonly SearchManager m_searchManager;
        private readonly ITextConverter m_textConverter;

        public SearchController(SearchManager searchManager, ITextConverter textConverter)
        {
            m_searchManager = searchManager;
            m_textConverter = textConverter;
        }

        [HttpPost("snapshot/count")]
        public FulltextSearchResultContract SearchByCriteriaCount([FromBody] SearchRequestContractBase searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }
            return m_searchManager.SearchByCriteriaCount(searchRequest);
        }

        [HttpPost("snapshot")]
        public FulltextSearchResultContract SearchByCriteria([FromBody] SearchRequestContractBase searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }
            return m_searchManager.SearchByCriteria(searchRequest);
        }

        [HttpPost("page/{resourceId}")]
        public TextResourceContract SearchPageByCriteria(string textResourceId, [FromQuery] TextFormatEnumContract formatValue, [FromBody] SearchPageRequestContract searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest), "Search request is null");
            }

            var textResource = m_searchManager.SearchPageByCriteria(textResourceId, searchRequest);
            textResource.Text = m_textConverter.Convert(textResource.Text, formatValue);

            return textResource;
        }


    }
}
