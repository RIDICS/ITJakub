using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentRenderer : HtmlObjectRenderer<CommentMark>
    {
        protected override void Write(HtmlRenderer renderer, CommentMark obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<span id=\"").Write(obj.CommentId).Write("-text\">").Write(obj.CommentContext).Write("</span>");
            }
        }
    }
}