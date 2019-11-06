namespace Vokabular.TextConverter.Markdown
{
    public interface IMarkdownToPlainTextConverter
    {
        string Convert(string text);
    }
}