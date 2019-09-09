namespace Vokabular.Shared.Converters
{
    public interface IMarkdownToHtmlConverter
    {
        string ConvertToHtml(string markdownText);
    }
}