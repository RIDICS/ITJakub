namespace Vokabular.FulltextService.Core.Helpers.Hml
{
    public interface IHtmlToPlainTextConverter
    {
        string ConvertToPlaintext(string htmlText);
    }
}