using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old
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