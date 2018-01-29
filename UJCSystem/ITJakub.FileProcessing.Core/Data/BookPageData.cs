using System.Collections.Generic;

namespace ITJakub.FileProcessing.Core.Data
{
    public class BookPageData
    {
        public string Text { get; set; }
        public string XmlId { get; set; }
        public string XmlResource { get; set; }
        public string Image { get; set; }
        public int Position { get; set; }
        public List<string> TermXmlIds { get; set; }
    }
}