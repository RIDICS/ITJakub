namespace Vokabular.FulltextService.Core.Helpers.Markdown
{
    public interface IMarkdownToPlainTextConverter
    {
        string Convert(string text);
    }
}