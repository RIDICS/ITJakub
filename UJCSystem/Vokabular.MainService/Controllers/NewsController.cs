using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class NewsController : BaseController
    {
        private readonly NewsManager m_newsManager;

        public NewsController(NewsManager newsManager)
        {
            m_newsManager = newsManager;
        }

        [HttpGet("")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public List<NewsSyndicationItemContract> GetNewsSyndicationItems([FromQuery] int? start, [FromQuery] int? count, [FromQuery] NewsTypeEnumContract? itemType)
        {
            var result = m_newsManager.GetNewsSyndicationItems(start, count, itemType);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpPost("")]
        public long CreateNewsSyndicationItem([FromBody] CreateNewsSyndicationItemContract data)
        {
            var resultId = m_newsManager.CreateNewsSyndicationItem(data);
            return resultId;
        }
    }
}
