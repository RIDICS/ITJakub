using System.Text;
using HtmlAgilityPack;

namespace Vokabular.FulltextService.Core.Helpers.Hml
{
    public class HtmlToPlainTextConverter : IHtmlToPlainTextConverter
    {
        public string ConvertToPlaintext(string htmlText)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);

            var builder = new StringBuilder();
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//text()"))
            {
                builder.Append(node.InnerText);
            }

            return builder.ToString();
        }
    }
}