using System.Collections.Generic;
using Markdig.Syntax.Inlines;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentMarkContainer : LeafInline
    {
        public string CommentId { get; set; }
        public List<Inline> ChildList { get; set; }
    }
}