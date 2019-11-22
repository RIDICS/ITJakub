using Markdig;
using Markdig.Renderers;

namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
{
    public class CommentMarkExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var parsers = pipeline.InlineParsers;

            if (!parsers.Contains<CommentMarkParser>())
            {
                parsers.Add(new CommentMarkParser());
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