namespace ITJakub.Web.Hub.Managers.Markdown
{
    public interface IMarkdownToHtmlConverter
    {
        string ConvertToHtml(string markdownText);
    }
}