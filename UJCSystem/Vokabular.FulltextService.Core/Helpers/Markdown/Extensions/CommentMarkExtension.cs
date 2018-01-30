using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Options;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentMarkExtension : IMarkdownExtension
    {
        private readonly IOptions<SpecialCharsOption> m_options;

        public CommentMarkExtension(IOptions<SpecialCharsOption> options)
        {
            m_options = options;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var parsers = pipeline.InlineParsers;

            if (!parsers.Contains<CommentMarkParser>())
            {
                parsers.Add(new CommentMarkParser(m_options));
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            var renderers = htmlRenderer?.ObjectRenderers;

            if (renderers != null && !renderers.Contains<CommentMarkRenderer>())
            {
                renderers.Add(new CommentMarkRenderer());
            }
        }
    }
}