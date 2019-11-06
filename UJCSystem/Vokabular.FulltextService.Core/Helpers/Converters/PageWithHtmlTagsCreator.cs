using System.Text.RegularExpressions;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Helpers.Converters
{
    public class PageWithHtmlTagsCreator : IPageWithHtmlTagsCreator
    {
        private readonly string m_pattern = "\\n";
        private readonly string m_replacement = "<br>";

        public string CreatePage(string textResourcePageText, TextFormatEnumContract formatValue)
        {
            if (formatValue != TextFormatEnumContract.Html)
            {
                return textResourcePageText;
            }

            textResourcePageText = ReplaceLineBreaks(textResourcePageText);
            textResourcePageText = AddPageTags(textResourcePageText);

            return textResourcePageText;
        }

        private string AddPageTags(string textResourcePageText)
        {
            textResourcePageText = textResourcePageText.Insert(0, "<div class=\"itj-page\">");
            textResourcePageText = string.Concat(new[] {textResourcePageText, "</div>"});

            return textResourcePageText;
        }

        private string ReplaceLineBreaks(string textResourcePageText)
        {
            var regex = new Regex(m_pattern);
            textResourcePageText = regex.Replace(textResourcePageText, m_replacement);

            return textResourcePageText;
        }
    }
}