namespace Vokabular.TextConverter.Converters
{
    public interface IMarkdownToHtmlConverter
    {
        string ConvertToHtml(string markdownText);
    }
}