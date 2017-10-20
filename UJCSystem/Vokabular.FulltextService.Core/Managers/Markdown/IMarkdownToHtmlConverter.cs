namespace Vokabular.FulltextService.Core.Managers.Markdown
{
    public interface IMarkdownToHtmlConverter
    {
        string ConvertToHtml(string markdownText);
    }
}