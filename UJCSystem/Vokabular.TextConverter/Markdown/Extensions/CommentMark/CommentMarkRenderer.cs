using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Vokabular.TextConverter.Markdown.Extensions.CommentMark
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
            renderer.Write($"<span data-text-reference-id=\"{commentMarkContainer.CommentId}\">"); 
            
            foreach (var child in commentMarkContainer.ChildList)
            {
                if (child is CommentMarkText)
                {
                    var castedchild = child as CommentMarkText;
                    renderer.Write(castedchild.Text);
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