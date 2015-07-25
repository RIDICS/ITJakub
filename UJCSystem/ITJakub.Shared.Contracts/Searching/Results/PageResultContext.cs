using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching.Results
{
    [DataContract]
    public class PageResultContext
    {
        [DataMember]
        public string PageXmlId { get; set; }   //TODO use PageDescriptionContract

        [DataMember]
        public string PageName { get; set; }

        [DataMember]
        public KwicStructure ContextStructure { get; set; }
    }
}