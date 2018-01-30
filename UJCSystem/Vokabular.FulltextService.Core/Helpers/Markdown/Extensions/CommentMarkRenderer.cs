using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Vokabular.FulltextService.Core.Helpers.Markdown.Extensions
{
    public class CommentMarkRenderer : HtmlObjectRenderer<CommentMarkContainer>
    {
        protected override void Write(HtmlRenderer renderer, CommentMarkContainer obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                WriteComment(renderer, obj);
            }
        }

        private void WriteComment(HtmlRenderer renderer, CommentMarkContainer commentMarkContainer)
        {
            if (commentMarkContainer == null) return;
            renderer.Write("<span id=\"").Write(commentMarkContainer.CommentId).Write("-text\">"); 
            
            foreach (var child in commentMarkContainer.ChildList)
            {
                if (child is CommentMark)
                {
                    var castedchild = child as CommentMark;
                    renderer.Write(castedchild.CommentContext);
                }
                else if (child is CommentMarkContainer)
                {
                    WriteComment(renderer, child as CommentMarkContainer);
                }
            }
            
            renderer.Write("</span>");
        }
    }
}