using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Contracts.Searching
{
    [DataContract]
    [KnownType(typeof(SearchResultWithKwicContext))]
    [KnownType(typeof(SearchResultWithXmlContext))]
    [KnownType(typeof(SearchResultWithHtmlContext))]
    [KnownType(typeof(SearchResultWithImageContext))]
    public class SearchResult: ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }
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

    public class SearchResultWithImageContext:SearchResult
    {
        [DataMember]
        public string Image { get; set; }//todo change type of image from string to stream...
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