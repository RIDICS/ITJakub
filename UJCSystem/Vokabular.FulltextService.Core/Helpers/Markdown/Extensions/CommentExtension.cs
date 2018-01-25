using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            OrderedList<InlineParser> parsers;

            parsers = pipeline.InlineParsers;

            if (!parsers.Contains<CommentParser>())
            {
                parsers.Add(new CommentParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            HtmlRenderer htmlRenderer;
            ObjectRendererCollection renderers;

            htmlRenderer = renderer as HtmlRenderer;
            renderers = htmlRenderer?.ObjectRenderers;

            if (renderers != null && !renderers.Contains<CommentRenderer>())
            {
                renderers.Add(new CommentRenderer());
            }
        }
    }
}