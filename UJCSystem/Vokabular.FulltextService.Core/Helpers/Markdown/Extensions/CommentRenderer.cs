using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentRenderer : HtmlObjectRenderer<Comment>
    {
        protected override void Write(HtmlRenderer renderer, Comment obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<span id=\"").Write(obj.CommentId).Write("-text\">").Write(obj.CommentText).Write("</span>");
            }
        }
    }
}