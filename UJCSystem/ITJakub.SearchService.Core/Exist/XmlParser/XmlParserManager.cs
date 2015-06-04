using System.Collections.Generic;
using System.IO;
using ITJakub.Shared.Contracts;

namespace ITJakub.SearchService.Core.Exist.XmlParser
{

    //TODO delete this Manager and other parsers and remove them from windsor castle config
    public class XmlParserManager
    {
        private readonly PageListParser m_pageListParser;
        private readonly BookContentParser m_bookContentParser;

        public XmlParserManager(PageListParser pageListParser, BookContentParser bookContentParser)
        {
            m_pageListParser = pageListParser;
            m_bookContentParser = bookContentParser;
        }

        public List<BookPageContract> ParsePageList(Stream pageListStream)
        {
            return m_pageListParser.Parse(pageListStream);
        }

        public List<BookContentItemContract> ParseBookContent(Stream bookContentStream)
        {
            return m_bookContentParser.Parse(bookContentStream);
        } 

    }
}