using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts.Searching.Results", Name = "PageDescriptionContract")]
    public class PageDescriptionContract
    {
        [DataMember]
        public string PageXmlId { get; set; }

        [DataMember]
        public string PageName { get; set; }
    }
}