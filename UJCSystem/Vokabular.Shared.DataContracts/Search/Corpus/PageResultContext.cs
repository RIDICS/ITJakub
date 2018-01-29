using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Corpus
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results", Name = "PageResultContext")]
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