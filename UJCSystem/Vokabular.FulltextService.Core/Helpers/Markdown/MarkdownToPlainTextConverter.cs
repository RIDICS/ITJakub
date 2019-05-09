using Vokabular.FulltextService.Core.Helpers.Hml;

namespace Vokabular.FulltextService.Core.Helpers.Markdown
{
    public class MarkdownToPlainTextConverter : IMarkdownToPlainTextConverter
    {
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;
        private readonly IHtmlToPlainTextConverter m_htmlToPLainTextConverter;

        public MarkdownToPlainTextConverter(IMarkdownToHtmlConverter markdownToHtmlConverter, IHtmlToPlainTextConverter htmlToPLainTextConverter)
        {
            m_markdownToHtmlConverter = markdownToHtmlConverter;
            m_htmlToPLainTextConverter = htmlToPLainTextConverter;
        }

        public string Convert(string text)
        {
            return m_htmlToPLainTextConverter.ConvertToPlaintext(m_markdownToHtmlConverter.ConvertToHtml(text));
        }
    }
}