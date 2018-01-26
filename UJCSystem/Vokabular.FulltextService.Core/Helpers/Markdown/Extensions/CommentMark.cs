using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentMark : LeafInline
    {
        public StringSlice CommentId { get; set; }
        public StringSlice CommentContext { get; set; }
    }
}