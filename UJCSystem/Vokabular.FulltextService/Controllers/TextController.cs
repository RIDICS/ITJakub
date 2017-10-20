using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class TextController : Controller
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<TextController>();
        private readonly TextResourceManager m_textResourceManager;

        public TextController(TextResourceManager textResourceManager)
        {
            m_textResourceManager = textResourceManager;
        }

        [HttpGet("{textResourceId}")]
        public TextResourceContract GetTextResource(string textResourceId)
        {
            var page = m_textResourceManager.GetTextResource(textResourceId);
            return page;
        }

        [HttpPost]
        public ResultContract CreateTextResource([FromBody] TextResourceContract textResource)
        {
            var result = m_textResourceManager.CreateTextResource(textResource);
            return result;
        }
    }
}