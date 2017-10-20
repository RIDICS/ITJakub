using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.Core.Managers.Markdown;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class TextController : Controller
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<TextController>();
        private readonly TextResourceManager m_textResourceManager;
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;

        public TextController(TextResourceManager textResourceManager, IMarkdownToHtmlConverter markdownToHtmlConverter)
        {
            m_textResourceManager = textResourceManager;
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }

        [HttpGet("{textResourceId}")]
        public TextResourceContract GetTextResource(string textResourceId, [FromQuery] int formatValue)
        {
            var textResource = m_textResourceManager.GetTextResource(textResourceId);
            
            switch (formatValue)
            {
                case 0:
                    break;
                case 1:
                    textResource.Text = m_markdownToHtmlConverter.ConvertToHtml(textResource.Text);
                    break;
                case 2:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(formatValue), formatValue, null);
            }
            return textResource;
        }

        [HttpPost]
        public ResultContract CreateTextResource([FromBody] TextResourceContract textResource)
        {
            var result = m_textResourceManager.CreateTextResource(textResource);
            return result;
        }
    }
}