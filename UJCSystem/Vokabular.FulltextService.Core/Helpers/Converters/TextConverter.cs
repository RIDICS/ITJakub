using System;
using Vokabular.Shared.DataContracts.Types;
using Vokabular.TextConverter.Markdown;

namespace Vokabular.FulltextService.Core.Helpers.Converters
{
    public class TextConverter : ITextConverter
    {
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;
        
        public TextConverter(IMarkdownToHtmlConverter markdownToHtmlConverter)
        {
            m_markdownToHtmlConverter = markdownToHtmlConverter;
        }

        public string Convert(string textResourceText, TextFormatEnumContract formatValue)
        {
            switch (formatValue)
            {
                case TextFormatEnumContract.Raw:
                    return textResourceText; 
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