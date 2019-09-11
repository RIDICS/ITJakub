using Vokabular.Shared.DataContracts.Types;
using Vokabular.TextConverter.Converters;

namespace Vokabular.TextConverter
{
    public class TextConverter : ITextConverter
    {
        private readonly IMarkdownToHtmlConverter m_markdownConverter;

        public TextConverter(IMarkdownToHtmlConverter markdownConverter)
        {
            m_markdownConverter = markdownConverter;
        }

        public string ConvertText(string text, TextFormatEnumContract outputFormat)
        {
            switch (outputFormat)
            {
                case TextFormatEnumContract.Html:
                {
                    return m_markdownConverter.ConvertToHtml(text);
                }
                default:
                {
                    return text;
                }
            }
        }
    }
}
