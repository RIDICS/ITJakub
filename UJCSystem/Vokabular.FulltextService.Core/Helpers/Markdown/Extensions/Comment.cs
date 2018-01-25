using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class Comment : LeafInline
    {
        public StringSlice CommentId { get; set; }
        public StringSlice CommentText { get; set; }
    }
}