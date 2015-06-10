using System.Runtime.Serialization;

namespace ITJakub.ITJakubService.DataContracts
{
    [DataContract]
    public class PageBookmarkContract
    {
        [DataMember]
        public string PageXmlId { get; set; }


        [DataMember]
        public int PagePosition { get; set; }
    }
}