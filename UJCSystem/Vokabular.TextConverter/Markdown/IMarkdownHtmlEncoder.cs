namespace Vokabular.TextConverter.Markdown
{
    public interface IMarkdownHtmlEncoder
    {
        string EscapeHtml(string content);
    }
}