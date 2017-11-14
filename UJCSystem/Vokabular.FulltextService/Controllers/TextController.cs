using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Helpers;
using Vokabular.FulltextService.Core.Helpers.Markdown;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class TextController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<TextController>();
        private readonly TextResourceManager m_textResourceManager;
        private readonly ITextConverter m_textConverter;

        public TextController(TextResourceManager textResourceManager, ITextConverter textConverter)
        {
            m_textResourceManager = textResourceManager;
            m_textConverter = textConverter;
        }

        [HttpGet("{textResourceId}")]
        public TextResourceContract GetTextResource(string textResourceId, [FromQuery] TextFormatEnumContract formatValue)
        {
            var textResource = m_textResourceManager.GetTextResource(textResourceId);
            textResource.Text = m_textConverter.Convert(textResource.Text, formatValue);
            
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