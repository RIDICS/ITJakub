using System.Runtime.Serialization;

namespace Vokabular.Shared.DataContracts.Search.Old.SearchDetail
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ITJakub.Shared.Contracts", Name = "ManuscriptContract")]
    public class ManuscriptContract
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Settlement { get; set; }

        [DataMember]
        public string Idno { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string OriginDate { get; set; }

        [DataMember]
        public string Repository { get; set; }
    }
}