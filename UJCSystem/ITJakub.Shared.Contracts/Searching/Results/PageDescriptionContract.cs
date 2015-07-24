using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
{
    [DataContract]
    public class PageDescriptionContract
    {
        [DataMember]
        public string PageXmlId { get; set; }

        [DataMember]
        public string PageName { get; set; }
    }
}