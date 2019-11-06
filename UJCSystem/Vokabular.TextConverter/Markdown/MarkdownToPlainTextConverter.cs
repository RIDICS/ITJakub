using Vokabular.TextConverter.Html;

namespace Vokabular.TextConverter.Markdown
{
    public class MarkdownToPlainTextConverter : IMarkdownToPlainTextConverter
    {
        private readonly IMarkdownToHtmlConverter m_markdownToHtmlConverter;
        private readonly IHtmlToPlainTextConverter m_htmlToPlainTextConverter;

        public MarkdownToPlainTextConverter(IMarkdownToHtmlConverter markdownToHtmlConverter, IHtmlToPlainTextConverter htmlToPlainTextConverter)
        {
            m_markdownToHtmlConverter = markdownToHtmlConverter;
            m_htmlToPlainTextConverter = htmlToPlainTextConverter;
        }

        public string Convert(string text)
        {
            return m_htmlToPlainTextConverter.ConvertToPlaintext(m_markdownToHtmlConverter.ConvertToHtml(text));
        }
    }
}