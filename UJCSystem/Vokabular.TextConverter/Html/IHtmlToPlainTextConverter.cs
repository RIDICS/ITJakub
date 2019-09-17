namespace Vokabular.TextConverter.Html
{
    public interface IHtmlToPlainTextConverter
    {
        string ConvertToPlaintext(string htmlText);
    }
}