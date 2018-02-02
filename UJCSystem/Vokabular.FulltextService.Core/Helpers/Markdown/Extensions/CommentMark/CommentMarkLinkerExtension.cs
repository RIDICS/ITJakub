using Markdig;
using Markdig.Helpers;
using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Options;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions.CommentMark
{
    public static class CommentMarkLinkerExtension
    {
        public static MarkdownPipelineBuilder UseCommentMarks(this MarkdownPipelineBuilder pipeline, IOptions<SpecialCharsOption> options)
        {
            OrderedList<IMarkdownExtension> extensions = pipeline.Extensions;

            if (!extensions.Contains<CommentMarkExtension>())
            {
                extensions.Add(new CommentMarkExtension(options));
            }

            return pipeline;
        }
    }
}