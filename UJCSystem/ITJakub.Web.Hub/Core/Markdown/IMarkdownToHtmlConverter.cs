namespace ITJakub.Web.Hub.Core.Markdown
{
    public interface IMarkdownToHtmlConverter
    {
        string ConvertToHtml(string markdownText);
    }
}