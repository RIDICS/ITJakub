using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Searching
{
    [DataContract]
    [KnownType(typeof(SearchResultWithKwicContext))]
    [KnownType(typeof(SearchResultWithXmlContext))]
    [KnownType(typeof(SearchResultWithHtmlContext))]
    public class SearchResult
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string OriginalXml { get; set; }

        [DataMember]
        public List<string> Categories { get; set; }
    }

    [DataContract]
    public class SearchResultWithXmlContext:SearchResult
    {
        [DataMember]
        public string XmlContext { get; set; }
    }

    [DataContract]
    public class SearchResultWithKwicContext : SearchResult
    {
        [DataMember]
        public KwicStructure Kwic { get; set; }
    }

    [DataContract]
    public class SearchResultWithHtmlContext:SearchResult
    {
        [DataMember]
        public string HtmlContext { get; set; }
    }

    [DataContract]
    public class KwicStructure
    {
        [DataMember]
        public string Before { get; set; }

        [DataMember]
        public string Match { get; set; }

        [DataMember]
        public string After { get; set; }
    }
}