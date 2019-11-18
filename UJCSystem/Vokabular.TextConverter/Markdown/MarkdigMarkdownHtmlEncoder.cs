using System.IO;
using Markdig.Renderers;

namespace Vokabular.TextConverter.Markdown
{
    public class MarkdigMarkdownHtmlEncoder : IMarkdownHtmlEncoder
    {
        public string EscapeHtml(string content)
        {
            using (var stringWriter = new StringWriter())
            {
                var htmlRenderer = new HtmlRenderer(stringWriter);
                htmlRenderer.WriteEscape(content);

                return stringWriter.ToString();
            }
        }
    }
}