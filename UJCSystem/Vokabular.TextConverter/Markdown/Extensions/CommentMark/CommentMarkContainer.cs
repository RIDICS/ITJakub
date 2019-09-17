using System.Collections.Generic;
using Markdig.Syntax.Inlines;

namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
{
    public class CommentMarkContainer : LeafInline
    {
        public string CommentId { get; set; }
        public List<Inline> ChildList { get; set; }
    }
}