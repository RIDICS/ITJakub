using System.Collections.Generic;
using System.Runtime.Serialization;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class BookInfoContract
    {
        [DataMember]
        public long BookId { get; set; }

        [DataMember]
        public string BookXmlId { get; set; }
        
        [DataMember]
        public string LastVersionXmlId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SubTitle { get; set; }

        [DataMember]
        public string Acronym { get; set; }

        [DataMember]
        public string BiblText { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string PublishDate { get; set; }

        [DataMember]
        public string PublishPlace { get; set; }

        [DataMember]
        public IList<BookPageContract> BookPages { get; set; }
    }
}