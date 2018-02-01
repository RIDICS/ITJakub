using System;
using Vokabular.FulltextService.Core.Helpers.Hml;
using Vokabular.FulltextService.Core.Helpers.Markdown;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Helpers.Converters
{
    public class TextConverter : ITextConverter
    {
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;
        private readonly IHtmlToPlainTextConverter m_htmlToPLainTextConverter;

        public TextConverter(IMarkdownToHtmlConverter markdownToHtmlConverter, IHtmlToPlainTextConverter htmlToPLainTextConverter)
        {
            m_markdownToHtmlConverter = markdownToHtmlConverter;
            m_htmlToPLainTextConverter = htmlToPLainTextConverter;
        }

        public string Convert(string textResourceText, TextFormatEnumContract formatValue)
        {
            switch (formatValue)
            {
                case TextFormatEnumContract.Raw:
                    return m_htmlToPLainTextConverter.ConvertToPlaintext(m_markdownToHtmlConverter.ConvertToHtml(textResourceText));
                case TextFormatEnumContract.Html:
                    return m_markdownToHtmlConverter.ConvertToHtml(textResourceText);
                case TextFormatEnumContract.Rtf:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(formatValue), formatValue, null);
            }
        }
    }
}