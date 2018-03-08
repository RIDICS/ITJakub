namespace Vokabular.FulltextService.Core.Helpers.Markdown
{
    public interface IMarkdownToHtmlConverter
    {
        string ConvertToHtml(string markdownText);
    }
}