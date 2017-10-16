using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
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