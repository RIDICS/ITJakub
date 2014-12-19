using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    public class SearchResultContract
    {
        [DataMember]
        public string BookGuid { get; set; }

        [DataMember]
        public string VersionId { get; set; }

        [DataMember]
        public string BookType { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }

        [DataMember]
        public string PublishPlace { get; set; }

        [DataMember]
        public string PublishDate { get; set; }

        [DataMember]
        public List<AuthorContract> Authors { get; set; }

        [DataMember]
        public PublisherContract Publisher { get; set; }

        [DataMember]
        public List<string> Keywords { get; set; }

        [DataMember]
        public string Copyright { get; set; }

        [DataMember]
        public int PageCount { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public string CreateTimeString { get; set; }

        [DataMember]
        public List<EditorContract> Editors { get; set; }

        [DataMember]
        public List<ManuscriptContract> Manuscripts { get; set; }
    }
}