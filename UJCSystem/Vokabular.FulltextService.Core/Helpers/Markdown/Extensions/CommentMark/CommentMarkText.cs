using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions.CommentMark
{
    public class CommentMarkText : LeafInline
    {
        public StringSlice Text { get; set; }
    }
}