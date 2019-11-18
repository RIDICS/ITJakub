using Vokabular.TextConverter.Markdown;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class QueriesBuilderFactory
    {
        private readonly IMarkdownHtmlEncoder m_markdownHtmlEncoder;

        public QueriesBuilderFactory(IMarkdownHtmlEncoder markdownHtmlEncoder)
        {
            m_markdownHtmlEncoder = markdownHtmlEncoder;
        }

        public QueriesBuilder Create(IndexType indexType)
        {
            return new QueriesBuilder(indexType, m_markdownHtmlEncoder);
        }
    }
}