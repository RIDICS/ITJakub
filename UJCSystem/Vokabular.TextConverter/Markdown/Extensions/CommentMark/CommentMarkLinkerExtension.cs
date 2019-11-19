using Markdig;
using Markdig.Helpers;

namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
{
    public static class CommentMarkLinkerExtension
    {
        public static MarkdownPipelineBuilder UseCommentMarks(this MarkdownPipelineBuilder pipeline)
        {
            OrderedList<IMarkdownExtension> extensions = pipeline.Extensions;

            if (!extensions.Contains<CommentMarkExtension>())
            {
                extensions.Add(new CommentMarkExtension());
            }

            return pipeline;
        }
    }
}